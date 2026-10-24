using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.Contracts;
using PlatformService.Data;
using PlatformService.Data.PrepInMemoryData;
using PlatformService.Data.Repos;
using PlatformService.Services.Abstractions;
using PlatformService.Services.Commands;
using PlatformService.Services.Dispatchers;
using PlatformService.Services.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddScoped<IDispatcher, Dispatcher>();

builder.Services.AddScoped<
    IRequestHandler<GetPlatformServicesQuery, IEnumerable<PlatformReadResponse>>,
    GetPlatformServicesQueryHandler>();

builder.Services.AddScoped<
    IRequestHandler<CreatePlatformCommand, int>,
    CreatePlatformCommandHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

var app = builder.Build();

DbSeeder.Initialize(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.MapGet("/platform-services", async (
    IDispatcher queryDispatcher, CancellationToken cancellationToken) =>
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
        ? Results.Created($"/api/platform/{result.Value}", new { Id = result.Value })
        : Results.BadRequest(result.Value);

}).ProducesProblem(StatusCodes.Status500InternalServerError)
.ProducesProblem(StatusCodes.Status400BadRequest)
.Produces<int>(StatusCodes.Status201Created);

app.UseHttpsRedirection();

app.Run();
