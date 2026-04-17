namespace sispark_api.Domain.Entities;

public class Tarifa
{
    public int IdTarifa { get; set; }
    public int IdTipoServicio { get; set; }
    public int IdTipoVehiculo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public int? FraccionMinutos { get; set; }
    public DateOnly FechaInicioVigencia { get; set; }
    public DateOnly? FechaFinVigencia { get; set; }
    public int IdEstado { get; set; }

    public TipoServicio? TipoServicio { get; set; }
    public TipoVehiculo? TipoVehiculo { get; set; }
}
