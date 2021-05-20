using System;
using System.Collections.Generic;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;
using TestEnvironment.Docker.ImageOperations;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Mongo
{
    public class MongoSingleReplicaSetContainer : Container, IMongoContainer
    {
        private readonly MongoSingleReplicaSetContainerParameters _parameters;

        public string ReplicaSetName => _parameters.ReplicaSetName;

#pragma warning disable SA1201 // Elements should appear in the correct order
        public MongoSingleReplicaSetContainer(MongoSingleReplicaSetContainerParameters containerParameters)
#pragma warning restore SA1201 // Elements should appear in the correct order
            : base(containerParameters) =>
            _parameters = containerParameters;

        public MongoSingleReplicaSetContainer(MongoSingleReplicaSetContainerParameters containerParameters, IDockerClient dockerClient)
            : base(containerParameters, dockerClient) =>
            _parameters = containerParameters;

        public MongoSingleReplicaSetContainer(MongoSingleReplicaSetContainerParameters containerParameters, IDockerClient dockerClient, ILogger? logger)
            : base(containerParameters, dockerClient, logger) =>
            _parameters = containerParameters;

        public MongoSingleReplicaSetContainer(MongoSingleReplicaSetContainerParameters containerParameters, IContainerApi containerApi, ImageApi imageApi, ILogger? logger)
            : base(containerParameters, containerApi, imageApi, logger) =>
            _parameters = containerParameters;

        public string GetDirectNodeConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : IP.Loopback.ToString();
            var port = _parameters.CustomReplicaSetPort ?? (IsDockerInDocker ? 27017 : Ports![27017]);

            return $@"mongodb://{hostname}:{port}/?connect=direct";
        }

        public string GetConnectionString()
        {
            var hostname = IsDockerInDocker ? IPAddress : IP.Loopback.ToString();
            var port = _parameters.CustomReplicaSetPort ?? (IsDockerInDocker ? 27017 : Ports![27017]);

            return $@"mongodb://{hostname}:{port}/?replicaSet={_parameters.ReplicaSetName}";
        }
    }
}
