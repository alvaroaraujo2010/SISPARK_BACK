using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace sispark_api.Infrastructure.Data;

public class SchemaBootstrapper(IServiceProvider serviceProvider, ILogger<SchemaBootstrapper> logger)
{
    public async Task EnsureAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SisparkDbContext>();
        var connection = (MySqlConnection)dbContext.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        var updates = new[]
        {
            new TableColumnDefinition("clientes", "id_usuario_creacion", "INT NULL"),
            new TableColumnDefinition("clientes", "fecha_actualizacion", "DATETIME NULL"),
            new TableColumnDefinition("clientes", "id_usuario_actualizacion", "INT NULL"),
            new TableColumnDefinition("vehiculos", "id_usuario_creacion", "INT NULL"),
            new TableColumnDefinition("vehiculos", "fecha_actualizacion", "DATETIME NULL"),
            new TableColumnDefinition("vehiculos", "id_usuario_actualizacion", "INT NULL")
        };

        foreach (var update in updates)
        {
            try
            {
                if (await ColumnExistsAsync(connection, update.TableName, update.ColumnName, cancellationToken))
                {
                    continue;
                }

                var commandText =
                    $"ALTER TABLE {update.TableName} ADD COLUMN {update.ColumnName} {update.ColumnSql};";

                await dbContext.Database.ExecuteSqlRawAsync(commandText, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "No fue posible aplicar un ajuste de esquema automatico en {Table}.{Column}.",
                    update.TableName,
                    update.ColumnName);
            }
        }

        await EnsureSingleOpenRecordConstraintAsync(dbContext, connection, cancellationToken);
    }

    private async Task EnsureSingleOpenRecordConstraintAsync(
        SisparkDbContext dbContext,
        MySqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string tableName = "registros_parqueo";
        const string markerColumn = "registro_abierto";
        const string uniqueIndexName = "uq_registro_abierto_por_vehiculo";

        try
        {
            if (!await ColumnExistsAsync(connection, tableName, markerColumn, cancellationToken))
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    """
                    ALTER TABLE registros_parqueo
                    ADD COLUMN registro_abierto TINYINT
                    GENERATED ALWAYS AS (
                        IF(fecha_salida IS NULL, 1, NULL)
                    ) STORED;
                    """,
                    cancellationToken);
            }

            if (!await IndexExistsAsync(connection, tableName, uniqueIndexName, cancellationToken))
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    """
                    CREATE UNIQUE INDEX uq_registro_abierto_por_vehiculo
                    ON registros_parqueo (id_vehiculo, registro_abierto);
                    """,
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "No fue posible aplicar la restriccion de concurrencia para registros_parqueo.");
        }
    }

    private static async Task<bool> ColumnExistsAsync(
        MySqlConnection connection,
        string tableName,
        string columnName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT COUNT(*)
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
              AND COLUMN_NAME = @columnName;
            """;
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@columnName", columnName);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) > 0;
    }

    private static async Task<bool> IndexExistsAsync(
        MySqlConnection connection,
        string tableName,
        string indexName,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT COUNT(*)
            FROM INFORMATION_SCHEMA.STATISTICS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
              AND INDEX_NAME = @indexName;
            """;
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@indexName", indexName);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) > 0;
    }

    private sealed record TableColumnDefinition(string TableName, string ColumnName, string ColumnSql);
}
