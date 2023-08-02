using Microsoft.EntityFrameworkCore;
using RockPaperScissors.Api.Middlewares;
using RockPaperScissors.BLL.Commands;
using RockPaperScissors.BLL.Services;
using RockPaperScissors.BLL.Services.Interfaces;
using RockPaperScissors.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));

builder.Services.AddTransient<IUserService, UserService>();

var connectionString = builder.Configuration.GetConnectionString("RockPaperScissorsDb");
builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<BusinessExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();