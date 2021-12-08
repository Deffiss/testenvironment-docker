using System;
using System.Collections.Generic;
using Docker.DotNet;
using Microsoft.Extensions.Logging;
using IP = System.Net.IPAddress;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public static class IDockerEnvironmentBuilderExtensions
    {
        public static FtpContainerParameters DefaultParameters => new("ftp", "admin", "admin")
        {
            ImageName = "stilliard/pure-ftpd",
            Tag = "hardened",
            EnvironmentVariables = new Dictionary<string, string>
            {
                ["PUBLICHOST"] = IP.Loopback.ToString(),
                ["FTP_USER_NAME"] = "admin",
                ["FTP_USER_PASS"] = "admin",
                ["FTP_USER_HOME"] = "/home/ftpusers/admin"
            },
            ContainerCleaner = new FtpContainerCleaner(),
            ContainerWaiter = new FtpContainerWaiter()
        };

        public static IDockerEnvironmentBuilder AddFtpContainer(
            this IDockerEnvironmentBuilder builder,
            Func<FtpContainerParameters, FtpContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters());
            builder.AddContainer(parameters, (p, d, l) => new FtpContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddFtpContainer(
            this IDockerEnvironmentBuilder builder,
            Func<FtpContainerParameters, IDockerClient, ILogger?, FtpContainerParameters> paramsBuilder)
        {
            var parameters = paramsBuilder(builder.GetDefaultParameters(), builder.DockerClient, builder.Logger);
            builder.AddContainer(parameters, (p, d, l) => new FtpContainer(FixEnvironmentVariables(p), d, l));

            return builder;
        }

        public static IDockerEnvironmentBuilder AddFtpContainer(
            this IDockerEnvironmentBuilder builder,
            string name,
            string ftpUserName,
            string ftpPassword,
            string imageName = "stilliard/pure-ftpd",
            string tag = "hardened",
            IDictionary<string, string>? environmentVariables = null,
            IDictionary<ushort, ushort>? ports = null,
            bool reuseContainer = false)
        {
            builder.AddFtpContainer(p => p with
            {
                Name = name,
                FtpUserName = ftpUserName,
                FtpPassword = ftpPassword,
                ImageName = imageName,
                Tag = tag,
                EnvironmentVariables = environmentVariables,
                Ports = ports,
                Reusable = reuseContainer
            });

            return builder;
        }

        private static FtpContainerParameters FixEnvironmentVariables(FtpContainerParameters p) =>
            p with
            {
                EnvironmentVariables = new Dictionary<string, string>
                {
                    ["FTP_USER_NAME"] = p.FtpUserName,
                    ["FTP_USER_PASS"] = p.FtpPassword,
                    ["FTP_USER_HOME"] = $"/home/ftpusers/{p.FtpUserName}"
                }.MergeDictionaries(p.EnvironmentVariables),
            };

        private static FtpContainerParameters GetDefaultParameters(this IDockerEnvironmentBuilder builder) =>
            builder.Logger switch
            {
                { } => DefaultParameters with
                {
                    ContainerWaiter = new FtpContainerWaiter(builder.Logger),
                    ContainerCleaner = new FtpContainerCleaner(builder.Logger)
                },
                null => DefaultParameters
            };
    }
}
