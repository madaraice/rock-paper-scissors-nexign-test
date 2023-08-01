using Microsoft.EntityFrameworkCore;
using RockPaperScissors.BLL.Extensions;
using RockPaperScissors.BLL.Model;
using RockPaperScissors.BLL.Queries;
using RockPaperScissors.DAL;

namespace RockPaperScissors.BLL.Handlers;

public class GetGameStatsHandler : IRequestHandler<GetGameStatsQuery, GetGameStatsResult>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetGameStatsHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    
    public async Task<GetGameStatsResult> Handle(GetGameStatsQuery request, CancellationToken cancellationToken)
    {
        var game = (await _applicationDbContext
                .Games
                .SingleOrDefaultAsync(g => g.Id == request.GameId, cancellationToken))
            .EnsureNotNull()
            .EnsureHasState(GameState.GameEnded);

        var usersStats = await _applicationDbContext
            .GameUsers
            .Where(gu => gu.GameId == game.Id)
            .Join(
                _applicationDbContext.TurnGameUsers,
                gameUser => gameUser.Id,
                turnGameUser => turnGameUser.GameUserId,
                (user, gameUser) => new UserStat
                {
                    UserId = user.UserId,
                    UserName = "",
                    RoundNumber = gameUser.RoundNumber,
                    Turn = (Turn) gameUser.Turn
                })
            .ToListAsync(cancellationToken);

        var statsByUser = usersStats.ToLookup(us => us.UserId);

        var firstUserStats = statsByUser[0]
            .Select((u, index) => new UserStatWithRoundResult
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Turn = u.Turn,
                RoundNumber = u.RoundNumber,
                UserWinRound = GetRoundResult(u.Turn, statsByUser[1].ElementAt(index).Turn)
            })
            .ToArray();

        var secondUserStats = statsByUser[1]
            .Select((u, index) => new UserStatWithRoundResult
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Turn = u.Turn,
                RoundNumber = u.RoundNumber,
                UserWinRound = GetRoundResult(u.Turn, statsByUser[0].ElementAt(index).Turn)
            })
            .ToArray();

        return new GetGameStatsResult
        {
            WinnerUserId = GetWinnerId(firstUserStats, secondUserStats),
            FirstUserStats = firstUserStats,
            SecondUserStats = secondUserStats
        };
    }

    private static RoundResult GetRoundResult(Turn firstUserTurn, Turn secondUserTurn)
    {
        if (firstUserTurn == secondUserTurn)
        {
            return RoundResult.Draw;
        }

        return firstUserTurn switch
        {
            Turn.Rock when secondUserTurn is Turn.Paper => RoundResult.Lose,
            Turn.Rock when secondUserTurn is Turn.Scissors => RoundResult.Win,

            Turn.Paper when secondUserTurn is Turn.Scissors => RoundResult.Lose,
            Turn.Paper when secondUserTurn is Turn.Rock => RoundResult.Win,
            
            Turn.Scissors when secondUserTurn is Turn.Rock => RoundResult.Lose,
            Turn.Scissors when secondUserTurn is Turn.Paper => RoundResult.Win,
            
            Turn.Unknown => throw new InvalidOperationException(),
            _ => throw new ArgumentOutOfRangeException(nameof(firstUserTurn), firstUserTurn, null)
        };
    }
    
    private static long? GetWinnerId(UserStatWithRoundResult[] firstUserStats, UserStatWithRoundResult[] secondUserStats)
    {
        var firstUserWinsCount = firstUserStats.Count(u => u.UserWinRound is RoundResult.Win);
        var secondUserWinsCount = secondUserStats.Count(u => u.UserWinRound is RoundResult.Win);

        if (firstUserWinsCount == secondUserWinsCount)
        {
            return null;
        }

        return firstUserWinsCount > secondUserWinsCount
            ? firstUserStats[0].UserId
            : secondUserStats[0].UserId;
    }
}