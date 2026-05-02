using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sispark_api.Application.Common;

namespace sispark_api.Infrastructure.Errors;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment hostEnvironment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Se produjo una excepcion no controlada.");

        var (statusCode, title, detail) = exception switch
        {
            ResourceNotFoundException resourceNotFound => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                resourceNotFound.Message),
            BusinessRuleException businessRule => (
                StatusCodes.Status400BadRequest,
                "Solicitud invalida",
                businessRule.Message),
            ConflictResourceException conflict => (
                StatusCodes.Status409Conflict,
                "Conflicto",
                conflict.Message),
            CredentialsRejectedException credentials => (
                StatusCodes.Status401Unauthorized,
                "Autenticacion fallida",
                credentials.Message),
            ArgumentException argument => (
                StatusCodes.Status400BadRequest,
                "Solicitud invalida",
                argument.Message),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Acceso no autorizado",
                "No tienes permisos para realizar esta operacion."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                "No se encontro el recurso solicitado."),
            InvalidOperationException => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                hostEnvironment.IsDevelopment()
                    ? exception.Message
                    : "Ocurrio un error inesperado. Intenta nuevamente."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                hostEnvironment.IsDevelopment()
                    ? exception.Message
                    : "Ocurrio un error inesperado. Intenta nuevamente."),
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
