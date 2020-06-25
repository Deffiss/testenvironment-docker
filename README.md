[![Build](https://github.com/Deffiss/testenvironment-docker/workflows/Build/badge.svg)](https://github.com/Deffiss/testenvironment-docker/actions?query=workflow%3ABuild)  [![NuGet](https://img.shields.io/nuget/v/TestEnvironment.Docker.svg)](https://www.nuget.org/packages/TestEnvironment.Docker/)

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
```
### Example

```csharp
// Create the environment using builder pattern.
var environment = new DockerEnvironmentBuilder()
    .AddContainer("my-nginx", "nginx")
    .AddElasticsearchContainer("my-elastic")
    .AddMssqlContainer("my-mssql", "HelloK11tt_0")
    .AddPostgresContainer("my-postgres")
    .AddFromDockerfile("from-file", "Dockerfile", containerWaiter: new HttpContainerWaiter("/", httpPort: 8080))
    .Build();

// Up it.
await environment.Up();

// Play with containers.
var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");
var postgres = environment.GetContainer<PostgresContainer>("my-postgres");

// Down it.
await environment.Down();

// Dispose (remove).
environment.Dispose();
```

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

* Implement `IAsyncDisposable` interface.
