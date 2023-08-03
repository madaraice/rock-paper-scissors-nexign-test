using Microsoft.EntityFrameworkCore;
using RockPaperScissors.BLL.Extensions;
using RockPaperScissors.BLL.Model;
using RockPaperScissors.BLL.Queries;
using RockPaperScissors.DAL;
using RockPaperScissors.DAL.Models;

namespace RockPaperScissors.BLL.Handlers;

public class GetStatsByRoundNumberHandler : IRequestHandler<GetStatsByRoundNumberQuery, GetStatsByRoundNumberResult>
{
    private readonly GameState[] _validGameStates = { GameState.GameInProgress, GameState.GameEnded };
    
    private readonly IApplicationDbContext _applicationDbContext;

    public GetStatsByRoundNumberHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<GetStatsByRoundNumberResult> Handle(
        GetStatsByRoundNumberQuery request,
        CancellationToken cancellationToken)
    {
        var game = (await _applicationDbContext
                .Games
                .SingleOrDefaultAsync(g => g.Id == request.GameId, cancellationToken))
            .EnsureNotNull()
            .EnsureHasStates(_validGameStates);

        var usersStatsByRound = await GetUsersStatsByRound(request, game, cancellationToken);
        
        var statsByUser = usersStatsByRound.ToLookup(us => us.UserId);
        var firstUserStats = statsByUser.First();
        var secondUserStats = statsByUser.Last();

        var firstUserStatsWithRound = GetUserInfoWithRoundResult(firstUserStats, secondUserStats);
        var secondUserStatsWithRound = GetUserInfoWithRoundResult(secondUserStats, firstUserStats);
        
        var winner = GetWinner(firstUserStatsWithRound, secondUserStatsWithRound);
        return new GetStatsByRoundNumberResult
        {
            WinnerUserId = winner?.UserId,
            WinnerUserName = winner?.UserName,
            FirstUserStats = firstUserStatsWithRound,
            SecondUserStats = secondUserStatsWithRound
        };
    }

    private Task<List<UserStat>> GetUsersStatsByRound(
        GetStatsByRoundNumberQuery request,
        Game game,
        CancellationToken cancellationToken)
    {
        return _applicationDbContext
            .GameUsers
            .Where(gu => gu.GameId == game.Id)
            .Join(
                _applicationDbContext.Users,
                gameUser => gameUser.UserId,
                user => user.Id,
                (gameUser, user) => new { UserId = gameUser.UserId, UserName = user.UserName, })
            .Join(
                _applicationDbContext.TurnGameUsers,
                userInfo => userInfo.UserId,
                turnGameUser => turnGameUser.GameUserId,
                (userInfo, turnGameUser) => new UserStat
                {
                    UserId = userInfo.UserId,
                    UserName = userInfo.UserName,
                    RoundNumber = turnGameUser.RoundNumber,
                    Turn = (Turn)turnGameUser.Turn
                })
            .Where(us => us.RoundNumber == request.RoundNumber)
            .ToListAsync(cancellationToken);
    }
    
        private static UserInfoWithRoundResult GetUserInfoWithRoundResult(
        IGrouping<long, UserStat> userStats,
        IGrouping<long, UserStat> opponentStats)
    {
        var userStat = userStats.First();

        return new UserInfoWithRoundResult
        {
            UserId = userStat.UserId,
            UserName = userStat.UserName,
            RoundResults = userStats
                .Select((u, index) => new UserInfoWithRoundResult.RoundResultByUser
                {
                    Turn = u.Turn,
                    RoundNumber = u.RoundNumber,
                    RoundResult = GetRoundResult(u.Turn, opponentStats.ElementAt(index).Turn)
                })
                .ToArray()
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