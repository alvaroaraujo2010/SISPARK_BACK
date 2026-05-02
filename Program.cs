using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using sispark_api.Application.Admin;
using sispark_api.Application.Auth;
using sispark_api.Application.Common;
using sispark_api.Application.Parking;
using sispark_api.Configuration;
using sispark_api.Infrastructure.Auth;
using sispark_api.Infrastructure.Data;
using sispark_api.Infrastructure.Errors;
using sispark_api.Infrastructure.Security;
using sispark_api.Services;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado.");
var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"];

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOptions<JwtOptions>()
    .Bind(configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Solicitud invalida",
                Detail = "Uno o mas campos no pasaron la validacion.",
                Instance = context.HttpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

            return new BadRequestObjectResult(problemDetails);
        };
    });
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SISPARK API",
        Version = "v1",
        Description = "API base para autenticacion, parqueo e integracion del sistema SISPARK."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT en formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<SisparkDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("SisparkDb")
        ?? throw new InvalidOperationException("ConnectionStrings:SisparkDb no configurada.");

    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36)));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("SisparkFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        AuthorizationPolicies.AdministrativeAccess,
        policy => policy.RequireRole(SystemRoles.Administrador, SystemRoles.Supervisor));

    options.AddPolicy(
        AuthorizationPolicies.ParkingOperation,
        policy => policy.RequireRole(SystemRoles.Administrador, SystemRoles.Operador, SystemRoles.Cajero));
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Demasiadas solicitudes",
                Detail = "Has superado el limite de intentos. Espera un momento e intentalo de nuevo.",
                Instance = context.HttpContext.Request.Path.Value,
            },
            cancellationToken);
    };

    options.AddPolicy(
        "auth-login",
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));
});

builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<SchemaBootstrapper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IParkingService, ParkingService>();
builder.Services.AddSingleton<IAppClock, SystemAppClock>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var schemaBootstrapper = scope.ServiceProvider.GetRequiredService<SchemaBootstrapper>();
    await schemaBootstrapper.EnsureAsync();

    if (configuration.GetValue("Database:ApplyMigrations", false))
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SisparkDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SISPARK API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "SISPARK API Docs";
    });
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("SisparkFrontend");
app.UseMiddleware<ApiSecurityHeadersMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();

app.Run();
