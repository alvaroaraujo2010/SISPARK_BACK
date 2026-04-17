namespace sispark_api.Domain.Entities;

public class TipoVehiculo
{
    public int IdTipoVehiculo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
