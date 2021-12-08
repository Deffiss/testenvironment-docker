using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace TestEnvironment.Docker.ImageOperations
{
    public class Archiver : IArchiver
    {
        private readonly ILogger? _logger;

        public Archiver()
        {
        }

        public Archiver(ILogger logger) =>
            _logger = logger;

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        public Task CreateTarArchiveAsync(string fileName, string directiry, string[]? ignoredFiles = default, CancellationToken cancellationToken = default)
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
        {
            using var stream = File.OpenWrite(fileName);
            using var writer = WriterFactory.Open(stream, ArchiveType.Tar, CompressionType.None);

            AddDirectoryFilesToTar(writer, fileName, directiry, directiry, true);

            return Task.CompletedTask;
        }

        // Adds recuresively files to tar archive.
#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        private void AddDirectoryFilesToTar(IWriter writer, string archiveFileName, string rootDirectory, string sourceDirectory, bool recurse, string[]? ignoredFiles = default)
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly
        {
            if (ignoredFiles?.Any(excl => excl.Equals(Path.GetFileName(sourceDirectory))) == true)
            {
                return;
            }

            // Write each file to the tar.
            var filenames = Directory.GetFiles(sourceDirectory);
            foreach (string filename in filenames)
            {
                if (Path.GetFileName(filename).Equals(archiveFileName))
                {
                    continue;
                }

                if (new FileInfo(filename).Attributes.HasFlag(FileAttributes.Hidden))
                {
                    continue;
                }

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
                    var contextDirectoryIndex = filename.IndexOf(rootDirectory);
                    var cleanPath = (contextDirectoryIndex < 0)
                        ? filename
                        : filename.Remove(contextDirectoryIndex, rootDirectory.Length);

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
                    AddDirectoryFilesToTar(writer, archiveFileName, rootDirectory, directory, recurse, ignoredFiles);
                }
            }
        }
    }
}
