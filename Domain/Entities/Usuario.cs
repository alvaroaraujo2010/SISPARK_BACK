namespace sispark_api.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string UsuarioLogin { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Contrasena { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public int IdEstado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    public Rol? Rol { get; set; }
    public Estado? Estado { get; set; }
}
