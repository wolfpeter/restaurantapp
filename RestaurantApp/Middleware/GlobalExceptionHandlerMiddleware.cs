using System.Net;
using System.Text.Json;

namespace RestaurantApp.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred. Request: {Method} {Path}", 
                context.Request.Method, context.Request.Path);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var statusCode = HttpStatusCode.InternalServerError;
        var errorMessage = "An unexpected error occurred.";
        string? errorDetails = null;

        switch (exception)
        {
            case ArgumentException or BadHttpRequestException:
                statusCode = HttpStatusCode.BadRequest;
                errorMessage = exception.Message;
                break;
            
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorMessage = exception.Message;
                break;
            
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorMessage = exception.Message;
                break;
        }

        response.StatusCode = (int)statusCode;

        if (_env.IsDevelopment())
        {
            errorDetails = exception.ToString(); 
        }

        var errorResponse = new
        {
            message = errorMessage,
            details = errorDetails
        };
        
        var result = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(result);
    }
}