using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Ftp
{
    public record FtpContainerParameters(string Name, string FtpUserName, string FtpPassword)
        : ContainerParameters(Name, "stilliard/pure-ftpd")
    {
    }
}
