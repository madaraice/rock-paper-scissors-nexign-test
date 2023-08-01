using MediatR;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Api.Models.Responses;
using RockPaperScissors.BLL.Commands;
using RockPaperScissors.BLL.Enums;
using RockPaperScissors.BLL.Queries;

namespace RockPaperScissors.Api.Controllers;

[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<GameController> _logger;

    public GameController(IMediator mediator, ILogger<GameController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("create")]
    public async Task<ActionResult<CreateGameResponse>> CreateGame(
        [FromQuery(Name = "userName")] string userName,
        CancellationToken cancellationToken)
    {
        var command = new CreateGameCommand { UserName = userName };
        var result = await _mediator.Send(command, cancellationToken);

        var response = new CreateGameResponse
        {
            GameId = result.GameId,
            UserId = result.UserId
        };
        return Ok(response);
    }

    [HttpGet]
    [Route("{gameId:long}/join/{userName}")]
    public async Task<ActionResult<JoinGameResponse>> JoinGame(
        [FromRoute] long gameId,
        [FromRoute] string userName,
        CancellationToken cancellationToken)
    {
        var command = new JoinGameCommand { GameId = gameId, UserName = userName };
        var result = await _mediator.Send(command, cancellationToken);

        var response = new JoinGameResponse
        {
            UserId = result.UserId
        };
        return Ok(response);
    }
    
    [HttpGet]
    [Route("{gameId:long}/user/{userId:long}/{turn}")]
    public async Task<IActionResult> MakeTurn(
        [FromRoute] long gameId,
        [FromRoute] long userId,
        [FromRoute] Turn turn,
        CancellationToken cancellationToken)
    {
        var command = new MakeTurnCommand { GameId = gameId, UserId = userId, Turn = turn };
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
    
    [HttpGet]
    [Route("{gameId:long}/stat")]
    public async Task<ActionResult<GetGameStatsResponse>> GetGameStats(
        [FromRoute] long gameId,
        CancellationToken cancellationToken)
    {
        var command = new GetGameStatsQuery { GameId = gameId };
        var result = await _mediator.Send(command, cancellationToken);

        var response = new GetGameStatsResponse
        {
            WinnerUserId = result.WinnerUserId,
            FirstUserStats = result.FirstUserStats,
            SecondUserStats = result.SecondUserStats
        };
        return Ok(response);
    }
}