using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Ftp
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public record FtpContainerParameters(string Name, string ImageName, string FtpUserName, string FtpPassword)
        : ContainerParameters(Name, ImageName)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
    }
}
