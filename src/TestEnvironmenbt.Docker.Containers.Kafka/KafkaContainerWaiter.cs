using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Kafka
{
    public class KafkaContainerWaiter : BaseContainerWaiter<KafkaContainer>
    {
        private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

        public KafkaContainerWaiter(ILogger logger = null)
            : base(logger)
        {
        }

        protected override async Task<bool> PerformCheck(KafkaContainer container, CancellationToken cancellationToken)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
                {
                    BootstrapServers = container.GetUrl(),
                })
                .Build();
            var topicName = $"{container.Name}_topic_health";
            try
            {
                while (true)
                {
                    var metadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(10));
                    if (metadata.Topics.Any(x => x.Topic == topicName) && await ProduceAMessage(topicName, container, cancellationToken))
                    {
                        return true;
                    }

                    var createPartitionTask = adminClient.CreateTopicsAsync(new[]
                    {
                        new TopicSpecification()
                        {
                            NumPartitions = 1,
                            Name = topicName,
                            ReplicationFactor = 1
                        },
                    });
                    await TimeoutAfter(createPartitionTask, TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Kafka initialization issue, retry after {RetryDelay.TotalSeconds} seconds");
                await Task.Delay(RetryDelay, cancellationToken);
                return false;
            }
        }

        private static async Task TimeoutAfter(Task task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)).ConfigureAwait(false))
            {
                cts.Cancel();
                await task.ConfigureAwait(false);
            }
            else
            {
                throw new OperationCanceledException($"Task timed out after {timeout.ToString()}");
            }
        }

        private async Task<bool> ProduceAMessage(string topicName, KafkaContainer container, CancellationToken cancellationToken)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = container.GetUrl()
            };
            using var p = new ProducerBuilder<string, string>(producerConfig).Build();
            try
            {
                await p.ProduceAsync(topicName, new Message<string, string> { Value = "test-message", Key = null }, cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Kafka produce issue");
                return false;
            }
        }
    }
}