using Microsoft.EntityFrameworkCore;
using RockPaperScissors.BLL.Extensions;
using RockPaperScissors.BLL.Services.Interfaces;
using RockPaperScissors.DAL;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Handlers;

public class JoinGameHandler : IRequestHandler<JoinGameCommand, JoinGameResult>
{
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
        // Костыль: было мало времени на настройку связей между таблицами и нужно было чтобы айдишники инкрементнулись
        // иначе бы в GameUsers записалось UserId 0
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var game = (await _applicationDbContext
                .Games
                .SingleOrDefaultAsync(g => g.Id == request.GameId, cancellationToken))
            .EnsureNotNull()
            .EnsureHasState(GameState.WaitingOpponentToJoin);

        _applicationDbContext.GameUsers.Add(new GameUser { UserId = user.Id, GameId = game.Id });
        
        game.State = (int) GameState.GameInProgress;
        _applicationDbContext.Games.Update(game);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new JoinGameResult { UserId = user.Id };
    }
}