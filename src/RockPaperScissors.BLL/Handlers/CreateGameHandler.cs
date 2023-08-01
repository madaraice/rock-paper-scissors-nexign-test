using RockPaperScissors.BLL.Services.Interfaces;
using RockPaperScissors.DAL;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Handlers;

public class CreateGameHandler : IRequestHandler<CreateGameCommand, CreateGameResult>
{
    private readonly IUserService _userService;
    private readonly IApplicationDbContext _applicationDbContext;

    public CreateGameHandler(IUserService userService, IApplicationDbContext applicationDbContext)
    {
        _userService = userService;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<CreateGameResult> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrAdd(request.UserName, cancellationToken);

        var utcNow = DateTimeOffset.UtcNow;
        var game = _applicationDbContext
            .Games
            .Add(new Game
            {
                State = (int) GameState.WaitingOpponentToJoin,
                CreatedAt = utcNow,
                UpdatedAt = utcNow
            })
            .Entity;

        _applicationDbContext.GameUsers.Add(new GameUser { UserId = user.Id, GameId = game.Id });

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new CreateGameResult
        {
            UserId = user.Id,
            GameId = game.Id
        };
    }
}