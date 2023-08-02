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
                _applicationDbContext.Users,
                gameUser => gameUser.UserId,
                user => user.Id,
                (gameUser, user) => new
                {
                    UserId = gameUser.UserId,
                    UserName = user.UserName,
                })
            .Join(
                _applicationDbContext.TurnGameUsers,
                userInfo => userInfo.UserId,
                turnGameUser => turnGameUser.GameUserId,
                (userInfo, turnGameUser) => new UserStat
                {
                    UserId = userInfo.UserId,
                    UserName = userInfo.UserName,
                    RoundNumber = turnGameUser.RoundNumber,
                    Turn = (Turn) turnGameUser.Turn
                })
            .OrderBy(us => us.RoundNumber)
            .ToListAsync(cancellationToken);

        var statsByUser = usersStats.ToLookup(us => us.UserId);
        var firstUserStats = statsByUser.First();
        var secondUserStats = statsByUser.Last();

        var firstUserStatsWithRounds = new UserInfoWithRoundResult
        {
            UserId = firstUserStats.First().UserId,
            UserName = firstUserStats.First().UserName,
            RoundResults = firstUserStats
                .Select((u, index) => new UserInfoWithRoundResult.RoundResultByUser
                {
                    Turn = u.Turn,
                    RoundNumber = u.RoundNumber,
                    RoundResult = GetRoundResult(u.Turn, secondUserStats.ElementAt(index).Turn)
                })
                .ToArray()
        };
        
        var secondUserStatsWithRounds = new UserInfoWithRoundResult
        {
            UserId = secondUserStats.First().UserId,
            UserName = secondUserStats.First().UserName,
            RoundResults = secondUserStats
                .Select((u, index) => new UserInfoWithRoundResult.RoundResultByUser
                {
                    Turn = u.Turn,
                    RoundNumber = u.RoundNumber,
                    RoundResult = GetRoundResult(u.Turn, firstUserStats.ElementAt(index).Turn)
                })
                .ToArray()
        };
        
        var winner = GetWinner(firstUserStatsWithRounds, secondUserStatsWithRounds);
        return new GetGameStatsResult
        {
            WinnerUserId = winner?.UserId,
            WinnerUserName = winner?.UserName,
            FirstUserStats = firstUserStatsWithRounds,
            SecondUserStats = secondUserStatsWithRounds
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
    
    private static UserInfoWithRoundResult? GetWinner(UserInfoWithRoundResult firstUserStats, UserInfoWithRoundResult secondUserStats)
    {
        var firstUserWinsCount = firstUserStats.RoundResults.Count(u => u.RoundResult is RoundResult.Win);
        var secondUserWinsCount = secondUserStats.RoundResults.Count(u => u.RoundResult is RoundResult.Win);

        if (firstUserWinsCount == secondUserWinsCount)
        {
            return null;
        }

        return firstUserWinsCount > secondUserWinsCount
            ? firstUserStats
            : secondUserStats;
    }
}