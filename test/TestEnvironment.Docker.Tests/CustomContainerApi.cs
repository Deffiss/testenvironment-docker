using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using TestEnvironment.Docker.ContainerOperations;

namespace TestEnvironment.Docker.Tests
{
    internal class CustomContainerApi : ContainerApi
    {
        private readonly string _namePrefix;

        public CustomContainerApi(string namePrefix, IDockerClient dockerClient, ILogger logger)
            : base(dockerClient, logger)
        {
            _namePrefix = namePrefix;
        }

        protected override CreateContainerParameters GetCreateContainerParameters(ContainerParameters containerParameters)
        {
            var createParams = base.GetCreateContainerParameters(containerParameters);

            createParams.Name = $"{createParams.Name}-{_namePrefix}";

            return createParams;
        }
    }
}
