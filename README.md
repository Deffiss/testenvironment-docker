[![Build status](https://ci.appveyor.com/api/projects/status/1xh2d15gkmij0mp8/branch/master?svg=true)](https://ci.appveyor.com/project/Deffiss/testenvironment-docker/branch/master)

# Test environment with Docker containers

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
