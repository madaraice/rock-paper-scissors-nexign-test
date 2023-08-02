using Microsoft.EntityFrameworkCore;
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

        // todo сделать тип игры типо против оппонента/PC
        var game = (await _applicationDbContext
                .Games
                .SingleOrDefaultAsync(g => g.Id == request.GameId, cancellationToken))
            .EnsureNotNull()
            .EnsureHasState(GameState.GameInProgress);

        var gameUser = (await _applicationDbContext
                .GameUsers
                .SingleOrDefaultAsync(gm => gm.GameId == game.Id && gm.UserId == user.Id, cancellationToken))
            .EnsureNotNull();

        var userTurns = await _applicationDbContext
            .TurnGameUsers
            .Where(tgu => tgu.GameUserId == gameUser.Id)
            .ToListAsync(cancellationToken);
        
        // todo проверить что юзер может делать ход (тип другой игрок уже походил)
        var opponentGameUser = await _applicationDbContext
            .GameUsers
            .SingleAsync(gm => gm.GameId == game.Id && gm.UserId != user.Id, cancellationToken);
        var opponentTurns = await _applicationDbContext
            .TurnGameUsers
            .Where(tgu => tgu.GameUserId == opponentGameUser.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        EnsureUserMayMakeTurn(
            userTurns.MaxBy(tgu => tgu.RoundNumber),
            opponentTurns.MaxBy(tgu => tgu.RoundNumber));
        
        var turnGameUser = new TurnGameUser
        {
            GameUserId = gameUser.Id,
            // todo глянуть автоинкремент через бд?
            RoundNumber = userTurns.Count + 1,
            Turn = (int) request.Turn,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _applicationDbContext.TurnGameUsers.Add(turnGameUser);

        // "+ 1" потому что в коллекции старый стейт ходов
        if (userTurns.Count + 1 == MaxRoundsCount
            && opponentTurns.Count == MaxRoundsCount)
        {
            // todo updatedAt
            game.State = (int) GameState.GameEnded;
            _applicationDbContext.Games.Update(game);
        }

        await _applicationDbContext.SaveChangesAsync(cancellationToken);
    }

    private void EnsureUserMayMakeTurn(TurnGameUser? userMaxRound, TurnGameUser? opponentMaxRound)
    {
        // Если это первый ход
        if (userMaxRound is null 
            && opponentMaxRound is null)
        {
            return;
        }
        
        // Если ОППОНЕНТ уже походил, а ЮЗЕР нет, то даем сделать ход 
        if (opponentMaxRound is not null 
            && userMaxRound is null)
        {
            return;
        }
        
        // Если ЮЗЕР уже походил, а ОППОНЕНТ нет, то НЕ даем сделать ход 
        if (userMaxRound is not null 
            && opponentMaxRound is null)
        {
            // todo rename
            throw new Exception();
        }
        
        // todo надо проверять на макс раунд?

        // Не даем юзеру сделать НОВЫЙ ход, если оппонент еще не сделал ход в ПРЕДЫДУЩЕМ раунде
        var difference = userMaxRound!.RoundNumber - opponentMaxRound!.RoundNumber;
        if (difference >= 1)
        {
            // todo rename
            throw new Exception();
        }
    }
}