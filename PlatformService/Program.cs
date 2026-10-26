using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.Contracts;
using PlatformService.Data;
using PlatformService.Data.PrepDb;
using PlatformService.Data.Repos;
using PlatformService.Services.Abstractions;
using PlatformService.Services.Commands;
using PlatformService.Services.Dispatchers;
using PlatformService.Services.Queries;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddScoped<IDispatcher, Dispatcher>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Logging.AddFilter(
    "System.Net.Http.HttpClient.ICommandDataClient",
    LogLevel.Warning);

builder.Services.AddScoped<
    IRequestHandler<GetPlatformServicesQuery, IEnumerable<PlatformReadResponse>>,
    GetPlatformServicesQueryHandler>();

builder.Services.AddScoped<
    IRequestHandler<CreatePlatformCommand, PlatformReadResponse>,
    CreatePlatformCommandHandler>();

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServiceConn")));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("InMemoryDb"));
}

var app = builder.Build();

DbSeeder.Initialize(app, builder.Environment.IsProduction());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.MapGet("/platform-services", async (
    IDispatcher queryDispatcher,
    CancellationToken cancellationToken) =>
{
    var query = new GetPlatformServicesQuery();
    var result = await queryDispatcher.SendAsync(query, cancellationToken);

    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Value);

}).ProducesProblem(StatusCodes.Status500InternalServerError)
.ProducesProblem(StatusCodes.Status400BadRequest)
.Produces<IEnumerable<PlatformReadResponse>>(StatusCodes.Status200OK);

app.MapPost("/platform-service", async (
    IDispatcher commandDispatcher,
    [FromBody] PlatformCreateRequest request,
    CancellationToken cancellationToken) =>
{
    var command = new CreatePlatformCommand(
        request.Name,
        request.Publisher,
        request.Cost);

    var result = await commandDispatcher
        .SendAsync(command, cancellationToken);

    return result.IsSuccess
        ? Results.Created((string?)null, result.Value)
        : Results.Problem(
            detail: result.Error?.Message,
            statusCode: result.Error?.Code == "Service.Unavailable"
                ? StatusCodes.Status503ServiceUnavailable
                : StatusCodes.Status400BadRequest);

}).ProducesProblem(StatusCodes.Status500InternalServerError)
.ProducesProblem(StatusCodes.Status400BadRequest)
.Produces<PlatformReadResponse>(StatusCodes.Status201Created);

app.Run();
