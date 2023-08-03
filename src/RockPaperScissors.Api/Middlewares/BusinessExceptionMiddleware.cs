using System.Net;
using System.Text.Json;
using RockPaperScissors.BLL.Exceptions;

namespace RockPaperScissors.Api.Middlewares;

public class BusinessExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BusinessExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (BusinessLogicException exception)
        {
            await HandleBusinessException(httpContext, exception);
        }
    }
    
    private Task HandleBusinessException(HttpContext context, BusinessLogicException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

        var errorDetails = new ErrorDetails
        {
            StatusCode = context.Response.StatusCode,
            Message = GetErrorMessage(exception)
        };
        var json = JsonSerializer.Serialize(errorDetails);

        return context.Response.WriteAsync(json);
    }

    private static string GetErrorMessage(BusinessLogicException exception)
    {
        return exception switch
        {
            GameNotFoundException => "Игра не найдена",
            GameHasInvalidStateException e => $"Для текущего действия ожидалось состояние игры {e.ExpectedState}, текущее состояние {e.ActualState}",
            GameHasInvalidStatesException e => $"Для текущего действия ожидалось одно из состояний игры: {string.Join(", ", e.ExpectedStates)}, текущее состояние {e.ActualState.ToString()}",
            UserNotFoundException => "Юзер не найден",
            UserNotJoinedInGameException => "Юзер не присоединился к переданной игре",
            WaitNextTurnException => "Жди следующего хода, оппонент еще не сделал ход",
            _ => "Необработанное исключение"
        };
    }
}