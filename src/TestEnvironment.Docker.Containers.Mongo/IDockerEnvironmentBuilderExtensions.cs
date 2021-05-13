using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static MongoContainerParameters DefaultMongoContainerParameters => new ("mongo", "root", "example")
        {
            ImageName = "mongo",
            ContainerCleaner = new MongoContainerCleaner(),
            ContainerWaiter = new MongoContainerWaiter()
        };

        public static MongoSingleReplicaSetContainerParameters DefaultMongoSingleReplicaSetContainerParameters => new ("replica", "rs0")
        {
            ImageName = "mongo",
            ContainerCleaner = new MongoContainerCleaner(),
            ContainerWaiter = new MongoSingleReplicaSetContainerWaiter(),
            ContainerInitializer = new MongoSingleReplicaSetContainerInitializer()
        };

        public static IDockerEnvironmentBuilder AddMongoContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MongoContainerParameters, MongoContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultMongoContainerParameters());
            builder.AddContainer(parameters, (p, d, l) => new MongoContainer(FixMongoEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddMongoContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MongoContainerParameters, IDockerClient, ILogger?, MongoContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultMongoContainerParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new MongoContainer(FixMongoEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddMongoSingleReplicaSetContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MongoSingleReplicaSetContainerParameters, MongoSingleReplicaSetContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultMongoSingleReplicaSetContainerParameters());
            builder.AddContainer(parameters, (p, d, l) => new MongoSingleReplicaSetContainer(FixtMongoSingleReplicaSetContainerParameters(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddMongoSingleReplicaSetContainer(
            this IDockerEnvironmentBuilder builder,
            Func<MongoSingleReplicaSetContainerParameters, IDockerClient, ILogger?, MongoSingleReplicaSetContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultMongoSingleReplicaSetContainerParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new MongoSingleReplicaSetContainer(FixtMongoSingleReplicaSetContainerParameters(p), d, l));

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddMongoSingleReplicaSetContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string replicaSetName = "rs0",
            string imageName = "mongo",
            string tag = "latest",
            ushort? port = null,
            IDictionary<string, string>? environmentVariables = null,
            bool reuseContainer = false)
        {
            builder.AddMongoSingleReplicaSetContainer(p => p with
            {
                Name = name,
                ReplicaSetName = replicaSetName,
                ImageName = imageName,
                Tag = tag,
                CustomReplicaSetPort = port,
                EnvironmentVariables = environmentVariables,
                Reusable = reuseContainer
            });

            return builder;
        }

        [Obsolete("This method is depricated and will be removed in upcoming versions.")]
        public static IDockerEnvironmentBuilder AddMongoContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string userName = "root",
            string userPassword = "example",
            string imageName = "mongo",
            string tag = "latest",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddMongoContainer(p => p with
            {
                Name = name,
                UserName = userName,
                Password = userPassword,
                ImageName = imageName,
                Tag = tag,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static MongoContainerParameters FixMongoEnvironmentVariables(MongoContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["MONGO_INITDB_ROOT_USERNAME"] = p.UserName,
                    ["MONGO_INITDB_ROOT_PASSWORD"] = p.Password
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static MongoSingleReplicaSetContainerParameters FixtMongoSingleReplicaSetContainerParameters(MongoSingleReplicaSetContainerParameters p) =>
            p with
            {
                Ports = p.CustomReplicaSetPort.HasValue
                    ? new Dictionary<ushort, ushort> { { p.CustomReplicaSetPort.Value, p.CustomReplicaSetPort.Value } }
                    : new Dictionary<ushort, ushort> { { 27017, 27017 } },
                ExposedPorts = p.CustomReplicaSetPort.HasValue
                    ? new List<ushort> { p.CustomReplicaSetPort.Value }
                    : null,
                Entrypoint = p.CustomReplicaSetPort.HasValue
                    ? new List<string> { "/usr/bin/mongod", "--bind_ip_all", "--replSet", p.ReplicaSetName, "--port", p.CustomReplicaSetPort.Value.ToString() }
                    : new List<string> { "/usr/bin/mongod", "--bind_ip_all", "--replSet", p.ReplicaSetName }
            };

        private static MongoContainerParameters GetDefaultMongoContainerParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultMongoContainerParameters with
                {
                    ContainerWaiter = new MongoContainerWaiter(builder.Logger),
                    ContainerCleaner = new MongoContainerCleaner(builder.Logger)
                },
                null => DefaultMongoContainerParameters
            };

        private static MongoSingleReplicaSetContainerParameters GetDefaultMongoSingleReplicaSetContainerParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultMongoSingleReplicaSetContainerParameters with
                {
                    ContainerWaiter = new MongoSingleReplicaSetContainerWaiter(builder.Logger),
                    ContainerCleaner = new MongoContainerCleaner(builder.Logger)
                },
                null => DefaultMongoSingleReplicaSetContainerParameters
            };
    }
}