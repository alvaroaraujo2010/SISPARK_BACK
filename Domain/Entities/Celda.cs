namespace sispark_api.Domain.Entities;

public class Celda
{
    public int IdCelda { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public int IdTipoVehiculo { get; set; }
    public int IdEstado { get; set; }
    public string? Observaciones { get; set; }
}
