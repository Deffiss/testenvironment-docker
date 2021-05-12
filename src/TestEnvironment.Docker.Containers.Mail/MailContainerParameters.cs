using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Mail
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record MailContainerParameters(string Name)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
       : ContainerParameters(Name, "mailhog/mailhog")
    {
        public ushort SmptPort { get; init; } = 1025;

        public ushort ApiPort { get; init; } = 8025;

        public string DeleteEndpoint { get; init; } = "api/v1/messages";
    }
}
