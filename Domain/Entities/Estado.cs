namespace sispark_api.Domain.Entities;

public class Estado
{
    public int IdEstado { get; set; }
    public string Modulo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
