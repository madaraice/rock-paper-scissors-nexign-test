using Microsoft.EntityFrameworkCore;
using RockPaperScissors.BLL.Exceptions;
using RockPaperScissors.BLL.Extensions;
using RockPaperScissors.BLL.Services.Interfaces;
using RockPaperScissors.DAL;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Handlers;

public class JoinGameHandler : IRequestHandler<JoinGameCommand, JoinGameResult>
{
    private const int MaxPlayersInGameCount = 2;
    
    private readonly IUserService _userService;
    private readonly IApplicationDbContext _applicationDbContext;

    public JoinGameHandler(IUserService userService, IApplicationDbContext applicationDbContext)
    {
        _userService = userService;
        _applicationDbContext = applicationDbContext;
    }
    
    public async Task<JoinGameResult> Handle(JoinGameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.GetOrAdd(request.UserName, cancellationToken);

        var game = (await _applicationDbContext
                .Games
                .SingleOrDefaultAsync(g => g.Id == request.GameId, cancellationToken))
            .EnsureNotNull()
            .EnsureHasState(GameState.WaitingOpponentToJoin);

        //await EnsureGameHasEmptySlot(game.Id, cancellationToken);

        _applicationDbContext.GameUsers.Add(new GameUser { UserId = user.Id, GameId = game.Id });
        
        game.State = (int) GameState.GameInProgress;
        _applicationDbContext.Games.Update(game);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new JoinGameResult { UserId = user.Id };
    }

    // todo remove
    private async Task EnsureGameHasEmptySlot(long gameId, CancellationToken cancellationToken)
    {
        var currentPlayersInGameCount = await _applicationDbContext
            .GameUsers
            .CountAsync(
                g => g.Id == gameId,
                cancellationToken);
        
        if (currentPlayersInGameCount == MaxPlayersInGameCount)
        {
            throw new GameDontHasEmptySlotException();
        }
    }
}