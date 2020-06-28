using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;
using TestEnvironment.Docker.DockerApi.Abstractions.Models;

namespace TestEnvironment.Docker.DockerApi.Internal
{
    public static class MappingExtensions
    {
        public static ContainerInfo ToContainerInfo(this ContainerListResponse response) => new ContainerInfo(
                response.ID,
                response.State,
                response.Status,
                response.NetworkSettings.Networks.FirstOrDefault().Key,
                response.NetworkSettings.Networks.FirstOrDefault().Value.IPAddress,
                response.Ports.ToDictionary(p => p.PrivatePort, p => p.PublicPort));

        public static CreateContainerParameters ToCreateContainerParameters(this ContainerConfiguration configuration) => new CreateContainerParameters
        {
            Name = configuration.Name,
            Image = $"{configuration.ImageName}:{configuration.Tag}",
            AttachStdout = true,
            Env = configuration.EnvironmentVariables.Select(p => $"{p.Key}={p.Value}").ToArray(),
            Hostname = configuration.Name,
            Domainname = configuration.Name,
            Entrypoint = configuration.EntryPoint,
            ExposedPorts = configuration.ExposedPorts?.ToDictionary(port => port, value => default(EmptyStruct)),
            HostConfig = new HostConfig
            {
                PublishAllPorts = configuration.Ports == null,
                PortBindings = configuration.Ports?.ToDictionary(p => $"{p.Key}/tcp", p => (IList<PortBinding>)new List<PortBinding> { new PortBinding { HostPort = p.Value.ToString() } })
            },
        };

        public static ImageBuildParameters ToImageBuildParameters(this ImageFromDockerfileConfiguration configuration) => new ImageBuildParameters
        {
            Dockerfile = configuration.Dockerfile,
            BuildArgs = configuration.BuildArgs ?? new Dictionary<string, string>(),
            Tags = new[] { $"{configuration.ImageName}:{configuration.Tag}" },
            PullParent = true,
            Remove = true,
            ForceRemove = true,
        };
    }
}
