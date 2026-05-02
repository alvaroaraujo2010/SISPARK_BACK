using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace sispark_api.Infrastructure.Data;

/// <summary>
/// Permite ejecutar <c>dotnet ef migrations add</c> / <c>dotnet ef database update</c> sin levantar la API.
/// Cadena por variable de entorno <c>SISPARK_DESIGN_TIME_CONNECTION</c> o valor por defecto local.
/// </summary>
public sealed class SisparkDbContextFactory : IDesignTimeDbContextFactory<SisparkDbContext>
{
    public SisparkDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SISPARK_DESIGN_TIME_CONNECTION")
            ?? "server=127.0.0.1;port=3306;database=sispark_v2_db;user=root;password=;TreatTinyAsBoolean=true";

        var optionsBuilder = new DbContextOptionsBuilder<SisparkDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 36)));

        return new SisparkDbContext(optionsBuilder.Options);
    }
}
