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

    private sealed record TableColumnDefinition(string TableName, string ColumnName, string ColumnSql);
}
