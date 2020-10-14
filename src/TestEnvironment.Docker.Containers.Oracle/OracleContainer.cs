using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Oracle
{
    public class OracleContainer : Container
    {
        private readonly string _userName;
        private readonly string _password;

        public OracleContainer(
            DockerClient dockerClient,
            string name,
            string userName,
            string password,
            string imageName = "wnameless/oracle-xe-11g-r2",
            string tag = "latest",
            IDictionary<string, string> environmentVariables = null,
            IDictionary<ushort, ushort> ports = null,
            bool isDockerInDocker = false,
            bool reuseContainer = false,
            IContainerWaiter containerWaiter = null,
            IContainerCleaner containerCleaner = null,
            ILogger logger = null)
            : base(
                  dockerClient,
                  name,
                  imageName,
                  tag,
                  environmentVariables,
                  ports,
                  isDockerInDocker,
                  reuseContainer,
                  containerWaiter,
                  containerCleaner,
                  logger)
        {
            _userName = userName;
            _password = password;
        }

        public string GetConnectionString()
        {
            var host = IsDockerInDocker ? IPAddress : IP.Loopback.ToString();
            var port = IsDockerInDocker ? 1521 : Ports[1521];
            return $"Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port})))(CONNECT_DATA=(SERVICE_NAME=XE)));User ID={_userName};Password={_password}";
        }
    }
}
