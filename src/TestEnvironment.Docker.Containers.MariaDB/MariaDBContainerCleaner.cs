﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using TestEnvironment.Docker.ContainerLifecycle;

namespace TestEnvironment.Docker.Containers.MariaDB
{
    public class MariaDBContainerCleaner : IContainerCleaner<MariaDBContainer>
    {
        private const string GetAllDatabasesCommand = "show databases;";
        private const string DropDatabaseCommand = "drop database {0}";

        private static readonly string[] SystemDatabases = { "information_schema", "mysql", "performance_schema" };

        private readonly ILogger? _logger;

        public MariaDBContainerCleaner()
        {
        }

        public MariaDBContainerCleaner(ILogger logger) =>
            _logger = logger;

        public async Task CleanupAsync(MariaDBContainer container, CancellationToken cancellationToken = default)
        {
            using var connection = new MySqlConnection(container.GetConnectionString());
            using var getDatabasesCommand = new MySqlCommand(GetAllDatabasesCommand, connection);

            await getDatabasesCommand.Connection!.OpenAsync();

            try
            {
                var reader = await getDatabasesCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var databaseName = reader.GetString(0);

                    if (SystemDatabases.All(dn => !dn.Equals(databaseName, StringComparison.OrdinalIgnoreCase)))
                    {
                        using (var dropConnection = new MySqlConnection(container.GetConnectionString()))
                        using (var dropCommand = new MySqlCommand(string.Format(DropDatabaseCommand, databaseName), dropConnection))
                        {
                            await dropConnection.OpenAsync();
                            await dropCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                _logger?.LogWarning($"Cleanup issue: {e.Message}");
            }
        }

        public Task CleanupAsync(Container container, CancellationToken cancellationToken = default)
            => CleanupAsync((MariaDBContainer)container, cancellationToken);
    }
}
