using MediatR;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Api.Models.Requests;
using RockPaperScissors.Api.Models.Responses;
using RockPaperScissors.BLL.Commands;

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

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<CreateGameResponse>> CreateGame(
        CreateGameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateGameCommand { UserName = request.UserName };
        var result = await _mediator.Send(command, cancellationToken);

        var response = new CreateGameResponse
        {
            GameId = result.GameId,
            UserId = result.UserId
        };
        return Ok(response);
    }
}