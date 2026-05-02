namespace sispark_api.Infrastructure.Security;

public sealed class ApiSecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (IsApiRequest(context.Request.Path))
        {
            var headers = context.Response.Headers;
            headers.TryAdd("X-Content-Type-Options", "nosniff");
            headers.TryAdd("X-Frame-Options", "DENY");
            headers.TryAdd("Referrer-Policy", "no-referrer");
            headers.TryAdd("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
            headers.TryAdd("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'; base-uri 'none';");
        }

        await next(context);
    }

    private static bool IsApiRequest(PathString path)
        => path.StartsWithSegments("/api");
}
