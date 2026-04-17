namespace sispark_api.Domain.Entities;

public class RegistroParqueo
{
    public int IdRegistro { get; set; }
    public int IdVehiculo { get; set; }
    public int IdTipoServicio { get; set; }
    public int IdTarifa { get; set; }
    public int? IdCelda { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaSalida { get; set; }
    public int? MinutosConsumidos { get; set; }
    public decimal ValorCalculado { get; set; }
    public decimal ValorFinal { get; set; }
    public int IdUsuarioIngreso { get; set; }
    public int? IdUsuarioSalida { get; set; }
    public int IdEstado { get; set; }
    public string? Observaciones { get; set; }

    public Vehiculo? Vehiculo { get; set; }
    public TipoServicio? TipoServicio { get; set; }
    public Tarifa? Tarifa { get; set; }
    public Celda? Celda { get; set; }
    public Usuario? UsuarioIngreso { get; set; }
    public Usuario? UsuarioSalida { get; set; }
}
