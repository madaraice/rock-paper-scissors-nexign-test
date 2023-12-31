﻿using RockPaperScissors.BLL.Services.Interfaces;
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

        var game = _applicationDbContext
            .Games
            .Add(new Game
            {
                State = (int) GameState.WaitingOpponentToJoin,
            })
            .Entity;

        // Костыль: было мало времени на настройку связей между таблицами и нужно было чтобы айдишники инкрементнулись
        // иначе бы в GameUsers записалось UserId 0 GameId 0
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        
        _applicationDbContext.GameUsers.Add(new GameUser { UserId = user.Id, GameId = game.Id });

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new CreateGameResult
        {
            UserId = user.Id,
            GameId = game.Id
        };
    }
}