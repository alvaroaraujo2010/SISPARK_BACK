namespace sispark_api.Domain.Entities;

public class Mensualidad
{
    public int IdMensualidad { get; set; }
    public int IdCliente { get; set; }
    public int IdVehiculo { get; set; }
    public int IdTarifa { get; set; }
    public int? IdCeldaFija { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public decimal Valor { get; set; }
    public int IdEstado { get; set; }
    public string? Observaciones { get; set; }

    public Cliente? Cliente { get; set; }
    public Vehiculo? Vehiculo { get; set; }
    public Tarifa? Tarifa { get; set; }
    public Celda? CeldaFija { get; set; }
}
