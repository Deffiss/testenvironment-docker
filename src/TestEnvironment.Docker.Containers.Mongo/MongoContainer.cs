using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoContainer : Container, IMongoContainer
    {
        private readonly MongoContainerParameters _parameters;

        public MongoContainer(MongoContainerParameters containerParameters)
            : base(containerParameters) =>
            _parameters = containerParameters;

        public MongoContainer(MongoContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public MongoContainer(MongoContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public MongoContainer(MongoContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public string GetConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : IP.Loopback.ToString();
            var port = IsDockerInDocker ? 27017 : Ports![27017];

            return string.IsNullOrEmpty(_parameters.UserName) || string.IsNullOrEmpty(_parameters.Password)
                ? $@"mongodb://{hostname}:{port}"
                : $@"mongodb://{_parameters.UserName}:{_parameters.Password}@{hostname}:{port}";
        }
    }
}