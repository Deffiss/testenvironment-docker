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
        private readonly string _key;
        private readonly string _val;

        public CustomContainerApi(string key, string val, IDockerClient dockerClient, ILogger logger)
            : base(dockerClient, logger)
        {
            _key = key;
            _val = val;
        }

        protected override CreateContainerParameters GetCreateContainerParameters(ContainerParameters containerParameters)
        {
            var createParams = base.GetCreateContainerParameters(containerParameters);

            createParams.Env.Add($"{_key}={_val}");

            return createParams;
        }
    }
}
