using CommandService.Contracts;
using CommandService.Data;
using CommandService.Data.Repos;
using CommandService.Endpoints;
using CommandService.Events;
using CommandService.Services.Commands;
using CommandService.Services.Queries.GetCommands;
using CommandService.Services.Queries.GetPlatforms;
using CommandService.Services.Subscriber;
using CommandService.Services.SyncDataServices;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Dispatchers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddScoped<IDispatcher, Dispatcher>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

builder.Services.AddScoped<
    IRequestHandler<GetAllPlatformsQuery, List<PlatformReadDto>>,
    GetAllPlatformsQueryHandler>();

builder.Services.AddScoped<
    IRequestHandler<GetCommandByPlatformIdQuery, CommandReadDto>,
    GetCommandByPlatformIdQueryHandler>();

builder.Services.AddScoped<
    IRequestHandler<GetCommandsForPlatformQuery, List<CommandReadDto>>,
    GetCommandsForPlatformQueryHandler>();

builder.Services.AddScoped<
    IRequestHandler<CreateCommandForPlatformCommand, CommandReadDto>,
    CreateCommandForPlatformCommandHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPlatformEndpoints();
app.MapCommandEndpoints();

await PrepDb.PrepPopulation(app);

app.Run();