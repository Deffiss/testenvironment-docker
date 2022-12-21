[![Build status](https://ci.appveyor.com/api/projects/status/1xh2d15gkmij0mp8/branch/master?svg=true)](https://ci.appveyor.com/project/Deffiss/testenvironment-docker/branch/master)  [![NuGet](https://img.shields.io/nuget/v/TestEnvironment.Docker.svg)](https://www.nuget.org/packages/TestEnvironment.Docker/)

# Test environment with Docker containers

### Pre requirements

You need docker to be installed on your machine. Tested both on Windows and Linux. 

### Installation

Run this in your package manager console:

```
 Install-Package TestEnvironment.Docker
```

To add container specific functionality for MSSQL, Elasticsearch or MongoDB:

```
 Install-Package TestEnvironment.Docker.Containers.Elasticsearch
 Install-Package TestEnvironment.Docker.Containers.Mssql
 Install-Package TestEnvironment.Docker.Containers.Mongo
 Install-Package TestEnvironment.Docker.Containers.Mail
 Install-Package TestEnvironment.Docker.Containers.Ftp
 Install-Package TestEnvironment.Docker.Containers.MariaDB
 Install-Package TestEnvironment.Docker.Containers.Postgres
 Install-Package TestEnvironment.Docker.Containers.Kafka
 Install-Package TestEnvironment.Docker.Containers.Redis
 Install-Package TestEnvironment.Docker.Containers.AzureSqlEdge
```
### Example
Latest version is heavily using C# 9 [records](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#record-types) feature. Please make sure you are targeting `net5.0` and above.

```csharp
// Create the environment using builder pattern.
await using var environment =  new DockerEnvironmentBuilder()
    .AddContainer(p => p with
    {
        Name = "my-nginx",
        ImageName = "nginx"
    })
    .AddElasticsearchContainer(p => p with
    {
        Name = "my-elastic"
    })
    .AddMssqlContainer(p => p with
    {
        Name = "my-mssql",
        SAPassword = "HelloK11tt_0"
    })
    .AddPostgresContainer(p => p with
    {
        Name = "my-postgres"
    })
    .AddFromDockerfile(p => p with
    {
        Name = "from-file",
        Dockerfile = "Dockerfile",
        ContainerWaiter = new HttpContainerWaiter("/", port: 8080)
    })
    .Build();

// Up it.
await environment.UpAsync();

// Play with containers.
var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");
var postgres = environment.GetContainer<PostgresContainer>("my-postgres");
```

### Docker Desktop subscription changes
As you may know, Docker announced subscription [changes](https://www.docker.com/blog/updating-product-subscriptions/). So you can not use Docker Desktop for free anymore (except for personal purposes). Good news is that Docker for Linux is still [free](https://www.docker.com/blog/looking-for-a-docker-alternative-consider-this/) (including Docker Engine, Docker Daemon, Docker CLI, Docker Compose, BuildKit, libraries, etc). So we have several options:

#### Option 1 - Run tests on Linux/WSL2
You can setup [WSL2](https://docs.microsoft.com/en-us/windows/wsl/install) on your Windows machine, install [.NET](https://docs.microsoft.com/en-us/dotnet/core/install/linux) and [Docker](https://docs.docker.com/engine/install/ubuntu/) (also, full instruction for Docker setup could be found [here](https://gist.github.com/Guddiny/e2cac200d8100a3926777109f770228b)). Then just navigate to your source code and simply run:

```
dotnet test
```

#### Option 2 - Expose Docker Daemon from WSL2 to Windows and proceed using Visual Studio
Ok, you still want to use Visual Studio to run and debug your tests. So this is still possible. First, you need to expose your Docker Daemon installed to WSL2. For this, go to `/etc/docker/` and create/edit `daemon.js` file:

```json
{
  "hosts": ["tcp://0.0.0.0:2375", "unix:///var/run/docker.sock"]
}
```

Then add `UseWsl2()` option to your tests:

```csharp
await using var environment =  new DockerEnvironmentBuilder()
    .UseWsl2()
    .AddMssqlContainer(p => p with
    {
        Name = "my-mssql",
        SAPassword = "HelloK11tt_0"
    })
```

And that's it!

### Podman support

As a container engine you can use [Podman](https://podman.io/) instead of docker. It works the same and even has the same api.
To install podman on Windows and use it follow this [instruction](https://gist.github.com/Guddiny/893555b398e86fc9c33bbeee41ff154a) (Super easy).

### Troubleshooting

In case of unpredictable behaviour try to remove the containers manually via command line:

```
docker rm -f (docker ps -a -q)
```

If you use **AddFromDockerfile()** then it is recommended to prune images time to time:

```
docker image prune -f
```

Ideally, use the [`--filter`](https://docs.docker.com/engine/reference/commandline/image_prune/#filtering) option on the `docker image prune` commandline task.  Simply add `LABEL "CI_BUILD=True"` in your Dockerfile, and force delete all images with that LABEL:

```
docker image prune -f --filter "CI_BUILD=True"
```

### Release Notes

* Add WSL2 Support
* Migrated to `net5.0` framework. Please reference 1.x.x packages if you need `netstandard2.0` support. This version will continue getting critical fixes in [branch](https://github.com/Deffiss/testenvironment-docker/tree/netstandard20).
