namespace sispark_api.Domain.Entities;

public class Vehiculo
{
    public int IdVehiculo { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public string? Color { get; set; }
    public int IdTipoVehiculo { get; set; }
    public int IdCliente { get; set; }
    public string? Observaciones { get; set; }
    public int IdEstado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int? IdUsuarioCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int? IdUsuarioActualizacion { get; set; }

    public TipoVehiculo? TipoVehiculo { get; set; }
    public Cliente? Cliente { get; set; }
}
