using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public string Name { get; }

        public IDictionary<string, string> Variables { get; }

        public IDependency[] Dependencies { get; }

        public DockerEnvironment(string name, IDictionary<string, string> variables, IDependency[] dependencies, DockerClient dockerClient, ILogger logger = null)
        {
            Name = name;
            Variables = variables;
            Dependencies = dependencies;
            _dockerClient = dockerClient;
            _logger = logger;
        }

        public async Task Up(CancellationToken token = default)
        {
            await BuildRequiredImages(token);

            await PullRequiredImages(token);

            await Task.WhenAll(Dependencies.Select(d => d.Run(Variables, token)));
        }

        public Task Down(CancellationToken token = default) =>
            Task.WhenAll(Dependencies.Select(d => d.Stop(token)));

        public Container GetContainer(string name) =>
            Dependencies.FirstOrDefault(d => d is Container c && c.Name.Equals(name.GetContainerName(Name), StringComparison.OrdinalIgnoreCase)) as Container;

        public TContainer GetContainer<TContainer>(string name) where TContainer : Container => GetContainer(name) as TContainer;

        public void Dispose()
        {
            foreach (var dependency in Dependencies)
            {
                dependency.Dispose();
            }
        }

        private async Task BuildRequiredImages(CancellationToken token)
        {
            foreach (var container in Dependencies.OfType<ContainerFromDockerfile>())
            {
                var tempFileName = Guid.NewGuid().ToString();

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
                    using (var fileStream = new FileStream(tempFileName, FileMode.CreateNew))
                    {
                        var tarArchive = TarArchive.CreateOutputTarArchive(fileStream);
                        var contextDirectory = container.Context.Equals(".") ? Directory.GetCurrentDirectory() : container.Context;

                        tarArchive.RootPath = contextDirectory.Replace('\\', '/');
                        if (tarArchive.RootPath.EndsWith("/"))
                        {
                            tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
                        }

                        AddDirectoryFilesToTar(tarArchive, contextDirectory, true);

                        tarArchive.Close();
                    }
                }

                // Adds recuresively files to tar archive.
                void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
                {
                    if (new[] { ".vs", ".vscode", "obj", "bin", ".git" }.Any(excl => excl.Equals(Path.GetFileName(sourceDirectory))))
                    {
                        return;
                    }

                    // Optionally, write an entry for the directory itself.
                    // Specify false for recursion here if we will add the directory's files individually.
                    var tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);
                    tarArchive.WriteEntry(tarEntry, false);

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
                            tarEntry = TarEntry.CreateEntryFromFile(filename);
                            tarArchive.WriteEntry(tarEntry, true);
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
                            AddDirectoryFilesToTar(tarArchive, directory, recurse);
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
    }
}
