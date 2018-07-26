using Docker.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public class DockerEnvironment : ITestEnvironment
    {
        private readonly DockerClient _dockerClient;

        public string Name { get; }

        public IDictionary<string, string> Variables { get; }

        public IDependency[] Dependencies { get; }

        public DockerEnvironment(string name, IDictionary<string, string> variables, IDependency[] dependencies, DockerClient dockerClient)
        {
            Name = name;
            Variables = variables;
            Dependencies = dependencies;
            _dockerClient = dockerClient;
        }

        public Task Up(CancellationToken token = default)
        {
            var environmentVariables = Variables.Select(p => (p.Key, p.Value)).ToArray();
            return Task.WhenAll(Dependencies.Select(d => d.Run(environmentVariables, token)));
        }

        public Task Down(CancellationToken token = default) =>
            Task.WhenAll(Dependencies.Select(d => d.Stop(token)));

        public Container GetContainer(string name) =>
            Dependencies.FirstOrDefault(d => d is Container c && c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) as Container;

        public TContainer GetContainer<TContainer>(string name) where TContainer : Container =>
            (TContainer)GetContainer(name);

        public void Dispose()
        {
            foreach (var dependency in Dependencies)
            {
                dependency.Dispose();
            }
        }
    }
}
