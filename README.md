[![Build status](https://ci.appveyor.com/api/projects/status/1xh2d15gkmij0mp8/branch/master?svg=true)](https://ci.appveyor.com/project/Deffiss/testenvironment-docker/branch/master)  [![NuGet](https://img.shields.io/nuget/v/TestEnvironment.Docker.svg)](https://www.nuget.org/packages/TestEnvironment.Docker/)

# Test environment with Docker containers

### Pre requirements

You need docker to be installed on your machine. Tested both on Windows and Linux. 

### Installation

Run this in your package manager console:

```
 Install-Package TestEnvironment.Docker
```

To add container specific functionality for MSSQL and Elasticsearch:

```
 Install-Package TestEnvironment.Docker.Containers.Elasticsearch
 Install-Package TestEnvironment.Docker.Containers.Mssql
```
### Example

```csharp
// Create the environment using builder pattern
var environment = new DockerEnvironmentBuilder()
    .AddContainer("my-nginx", "nginx")
    .AddElasticsearchContainer("my-elastic")
    .AddMssqlContainer("my-mssql", "HelloK11tt_0")
    .Build();

// Up it.
await environment.Up();

// Play with containers.
var mssql = environment.GetContainer<MssqlContainer>("my-mssql");
var elastic = environment.GetContainer<ElasticsearchContainer>("my-elastic");

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
