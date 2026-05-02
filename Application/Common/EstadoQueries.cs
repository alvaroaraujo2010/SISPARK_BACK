using Microsoft.EntityFrameworkCore;
using sispark_api.Infrastructure.Data;

namespace sispark_api.Application.Common;

internal static class EstadoQueries
{
    public static async Task<int> GetStateIdAsync(
        SisparkDbContext dbContext,
        string modulo,
        string nombre,
        CancellationToken cancellationToken)
    {
        var stateId = await dbContext.Estados
            .AsNoTracking()
            .Where(item => item.Modulo == modulo && item.Nombre == nombre)
            .Select(item => item.IdEstado)
            .FirstOrDefaultAsync(cancellationToken);

        if (stateId == 0)
        {
            throw new InvalidOperationException($"No se encontro el estado '{modulo}:{nombre}'.");
        }

        return stateId;
    }
}
