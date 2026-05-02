namespace sispark_api.Infrastructure.Auth;

public static class AuthorizationPolicies
{
    public const string AdministrativeAccess = "AdministrativeAccess";
    public const string ParkingOperation = "ParkingOperation";
}

public static class SystemRoles
{
    public const string Administrador = "Administrador";
    public const string Cajero = "Cajero";
    public const string Operador = "Operador";
    public const string Supervisor = "Supervisor";
}
