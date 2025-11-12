using System.Net;
using System.Text.Json;
using TodoList.Domain.Exceptions;

namespace TodoList.API.Middleware;

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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        object errorResponse = exception switch
        {
            TodoItemNotFoundException notFoundEx => new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = notFoundEx.Message,
                todoItemId = notFoundEx.TodoItemId
            },
            ValidationException validationEx => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = validationEx.Message,
                errors = validationEx.Errors
            },
            InvalidTodoOperationException invalidOpEx => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = invalidOpEx.Message
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "Ocurrió un error interno en el servidor. Por favor, inténtelo de nuevo más tarde."
            }
        };

        response.StatusCode = exception switch
        {
            TodoItemNotFoundException => (int)HttpStatusCode.NotFound,
            ValidationException => (int)HttpStatusCode.BadRequest,
            InvalidTodoOperationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

