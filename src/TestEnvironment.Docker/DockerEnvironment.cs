using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestEnvironment.Docker
{
    public class DockerEnvironment : ITestEnvironment
    {
        private readonly DockerClient _dockerClient;
        private readonly string[] _ignoredFiles;
        private readonly ILogger _logger;
        private readonly Dictionary<string, string> _dependenciesGraph;

        public string Name { get; }

        public IDictionary<string, string> Variables { get; }

        public IDependency[] Dependencies { get; }

        public DockerEnvironment(
            string name,
            IDictionary<string, string> variables,
            IDependency[] dependencies,
            DockerClient dockerClient,
            Dictionary<string, string> dependenciesGraph,
            string[] ignoredFiles = null,
            ILogger logger = null)
        {
            Name = name;
            Variables = variables;
            Dependencies = dependencies;
            _dockerClient = dockerClient;
            _dependenciesGraph = dependenciesGraph;
            _ignoredFiles = ignoredFiles;
            _logger = logger;
        }

        public async Task Up(CancellationToken token = default)
        {
            await BuildRequiredImages(token);

            await PullRequiredImages(token);

            var containerNames = Dependencies.Select(x => x.Name).ToList();

            while (containerNames.Count > 0)
            {
                // Find all containers that are not using other containers as dependency
                var independentContainerNames = containerNames.Where(name => !_dependenciesGraph.ContainsKey(name)).ToList();
                if (independentContainerNames.Count == 0)
                {
                    throw new Exception("Containers dependencies graph contains circular dependency!");
                }

                // Process independent containers
                await Task.WhenAll(Dependencies.Where(x => independentContainerNames.Contains(x.Name)).Select(d => d.Run(Variables, token)));

                // Remove processed containers from dependencies graph
                independentContainerNames.ForEach(name =>
                {
                    var waitingContainerNames = _dependenciesGraph.Where(kv => kv.Value == name).Select(kv => kv.Key).ToList();
                    if (waitingContainerNames.Any())
                    {
                        waitingContainerNames.ForEach(key => _dependenciesGraph.Remove(key));
                    }                    
                });
                containerNames = containerNames.Except(independentContainerNames).ToList();
            }
        }

        public Task Down(CancellationToken token = default) =>
            Task.WhenAll(Dependencies.Select(d => d.Stop(token)));

        public Container GetContainer(string name) =>
            Dependencies.FirstOrDefault(d => d is Container c && c.Name.Equals(name.GetContainerName(Name), StringComparison.OrdinalIgnoreCase)) as Container;

        public TContainer GetContainer<TContainer>(string name) where TContainer : Container => GetContainer(name) as TContainer;

        private async Task BuildRequiredImages(CancellationToken token)
        {
            foreach (var container in Dependencies.OfType<ContainerFromDockerfile>())
            {
                var tempFileName = Guid.NewGuid().ToString();
                var contextDirectory = container.Context.Equals(".") ? Directory.GetCurrentDirectory() : container.Context;

                try
                {
                    // In order to pass the context we have to create tar file and use it as an argument.
                    CreateTarArchive();
                
                    // Now call docker api.
                    await CreateImage();
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"Unable to create the image from dockerfile.");
                    throw;
                }

                // And don't forget to remove created tar.
                try
                {
                    File.Delete(tempFileName);
                }
                catch (Exception exc)
                {
                    _logger?.LogError(exc, $"Unable to delete tar file {tempFileName} with context. Please, cleanup manually.");
                }


                void CreateTarArchive()
                {
                    using (var stream = File.OpenWrite(tempFileName))
                    using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, CompressionType.None))
                    {
                        AddDirectoryFilesToTar(writer, contextDirectory, true);
                    }
                }

                // Adds recuresively files to tar archive.
                void AddDirectoryFilesToTar(IWriter writer, string sourceDirectory, bool recurse)
                {
                    if (_ignoredFiles?.Any(excl => excl.Equals(Path.GetFileName(sourceDirectory))) == true)
                    {
                        return;
                    }

                    // Write each file to the tar.
                    var filenames = Directory.GetFiles(sourceDirectory);
                    foreach (string filename in filenames)
                    {
                        if (Path.GetFileName(filename).Equals(tempFileName)) continue;
                        if (new FileInfo(filename).Attributes.HasFlag(FileAttributes.Hidden)) continue;

                        // Make sure that we can read the file
                        try
                        {
                            File.OpenRead(filename);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        try
                        {
                            var contextDirectoryIndex = filename.IndexOf(contextDirectory);
                            var cleanPath = (contextDirectoryIndex < 0)
                                ? filename
                                : filename.Remove(contextDirectoryIndex, contextDirectory.Length);

                            writer.Write(cleanPath, filename);
                        }
                        catch (Exception exc)
                        {
                            _logger?.LogWarning($"Can not add file {filename} to the context: {exc.Message}.");
                        }
                    }

                    if (recurse)
                    {
                        var directories = Directory.GetDirectories(sourceDirectory);
                        foreach (var directory in directories)
                        {
                            AddDirectoryFilesToTar(writer, directory, recurse);
                        }
                    }
                }

                async Task CreateImage()
                {
                    using (var tarContextStream = new FileStream(tempFileName, FileMode.Open))
                    {
                        var image = await _dockerClient.Images.BuildImageFromDockerfileAsync(tarContextStream, new ImageBuildParameters
                        {
                            Dockerfile = container.Dockerfile,
                            BuildArgs = container.BuildArgs ?? new Dictionary<string, string>(),
                            Tags = new[] { $"{container.ImageName}:{container.Tag}" },
                            PullParent = true,
                            Remove = true,
                            ForceRemove = true,
                        }, token);

                        await new StreamReader(image).ReadToEndAsync();
                    }
                }
            }
        }

        private async Task PullRequiredImages(CancellationToken token)
        {
            foreach(var contianer in Dependencies.OfType<Container>())
            {
                var images = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters
                {
                    All = true,
                    MatchName = $"{contianer.ImageName}:{contianer.Tag}"
                }, token);

                if (!images.Any())
                {
                    _logger.LogInformation($"Pulling the image {contianer.ImageName}:{contianer.Tag}");

                    // Pull the image.
                    try
                    {

                        await _dockerClient.Images.CreateImageAsync(
                            new ImagesCreateParameters
                            {
                                FromImage = contianer.ImageName,
                                Tag = contianer.Tag
                            }, null, new Progress<JSONMessage>(m => _logger.LogDebug($"Pulling image {contianer.ImageName}:{contianer.Tag}:\n{m.ProgressMessage}")), token);
                    }
                    catch (Exception exc)
                    {
                        _logger?.LogError(exc, $"Unable to pull the image {contianer.ImageName}:{contianer.Tag}");
                        throw;
                    }

                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var dependency in Dependencies)
                {
                    dependency.Dispose();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                var disposeTasks = Dependencies.Select(d => d.DisposeAsync()).ToArray();
                foreach (var dt in disposeTasks)
                {
                    await dt;
                }
            }
        }
    }
}
