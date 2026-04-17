namespace sispark_api.Domain.Entities;

public class Cliente
{
    public int IdCliente { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public int IdEstado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int? IdUsuarioCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public int? IdUsuarioActualizacion { get; set; }
}
