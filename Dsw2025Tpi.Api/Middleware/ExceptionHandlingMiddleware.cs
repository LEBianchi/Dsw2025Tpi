using Dsw2025Tpi.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Dsw2025Tpi.Api.NewFolder
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Se capturó una excepción no controlada: {ExceptionType}", ex.GetType().Name);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;

            switch (exception)
            {
                // Unificamos todas las excepciones que resultan en 400 Bad Request
                case InvalidOrderDataException or
                     InvalidDataException or
                     InvalidOrderStatusException or
                     DuplicatedEntityException or
                     ArgumentException or
                     InsufficientStockException or
                     ArgumentNullException: // ArgumentNullException también suele ser un bad request
                    status = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                // Todas las excepciones que resultan en 404 Not Found
                case CustomerNotFoundException or
                     OrderNotFoundException or
                     KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;

                // Caso específico para errores de autenticación/autorización
                case UnauthorizedAccessException:
                    status = HttpStatusCode.Forbidden; // 403 Forbidden
                    message = "No tienes permiso para acceder a este recurso.";
                    break;

               

                // Manejador por defecto para cualquier otra excepción
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = "Ocurrió un error inesperado en el servidor.";
                    break;
            }

            var result = JsonSerializer.Serialize(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}