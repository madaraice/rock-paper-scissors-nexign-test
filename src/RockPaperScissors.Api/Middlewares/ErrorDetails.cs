namespace RockPaperScissors.Api.Middlewares;

public class ErrorDetails
{
    public required int StatusCode { get; init; }
    public required string Message { get; init; }
}