using System.Net;
using System.Text.Json;
using FluentValidation;

namespace RockPaperScissors.Api.Middlewares;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ValidationException exception)
        {
            await HandleBusinessException(httpContext, exception);
        }
    }
    
    private Task HandleBusinessException(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

        var errorDetails = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = "Валидация запроса не была пройдена"
        };
        var json = JsonSerializer.Serialize(errorDetails);

        return context.Response.WriteAsync(json);
    }
}