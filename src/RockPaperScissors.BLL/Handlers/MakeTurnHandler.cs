using Microsoft.EntityFrameworkCore;
using RockPaperScissors.BLL.Exceptions;
using RockPaperScissors.BLL.Extensions;
using RockPaperScissors.DAL;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Handlers;

public class MakeTurnHandler : IRequestHandler<MakeTurnCommand>
{
    private const int MaxRoundsCount = 5;

    private readonly IApplicationDbContext _applicationDbContext;

    public MakeTurnHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task Handle(MakeTurnCommand request, CancellationToken cancellationToken)
    {
        var user = (await _applicationDbContext
                .Users
                .SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken))
            .EnsureNotNull();

        // todo сделать тип игры типо против оппонента
        var game = (await _applicationDbContext
                .Games
                .SingleOrDefaultAsync(g => g.Id == request.GameId, cancellationToken))
            .EnsureNotNull();

        var gameUser = (await _applicationDbContext
                .GameUsers
                .SingleOrDefaultAsync(gm => gm.GameId == game.Id && gm.UserId == user.Id, cancellationToken))
            .EnsureNotNull();

        var userTurns = await _applicationDbContext
            .TurnGameUsers
            .Where(tgu => tgu.GameUserId == gameUser.Id)
            .ToListAsync(cancellationToken);
        EnsureGameNotEnded(userTurns);
        
        // todo проверить что юзер может делать ход (тип другой игрок уже походил)
        var opponentGameUser = await _applicationDbContext
            .GameUsers
            .SingleOrDefaultAsync(gm => gm.GameId == game.Id && gm.UserId != user.Id, cancellationToken);
        var opponentTurns = await _applicationDbContext
            .TurnGameUsers
            .Where(tgu => tgu.GameUserId == opponentGameUser.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var turnGameUser = new TurnGameUser
        {
            GameUserId = gameUser.Id,
            // todo глянуть автоинкремент через бд?
            TurnNumber = userTurns.Count + 1,
            TurnValue = (int) request.Turn,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await _applicationDbContext.TurnGameUsers.AddAsync(turnGameUser, cancellationToken);
    }

    private static void EnsureGameNotEnded(List<TurnGameUser> userTurns)
    {
        if (userTurns.Count + 1 == MaxRoundsCount)
        {
            throw new GameAlreadyEndedException();
        }
    }
}