using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEnvironment.Docker.Containers.Redis
{
    public record RedisContainerParameters(string Name, string Password)
        : ContainerParameters(Name, "bitnami/redis")
    {
    }
}
