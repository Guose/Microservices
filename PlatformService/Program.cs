using Microsoft.EntityFrameworkCore;
using PlatformService.Contracts;
using PlatformService.Data;
using PlatformService.Data.PrepInMemoryData;
using PlatformService.Data.Repos;
using PlatformService.Services.Abstractions;
using PlatformService.Services.Commands;
using PlatformService.Services.Dispatchers;
using PlatformService.Services.Queries;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();
builder.Services.AddScoped<IQueryHandler<GetPlatformServicesQuery, IEnumerable<PlatformReadResponse>>, GetPlatformServicesQueryHandler>();

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
    IQueryDispatcher queryDispatcher, CancellationToken cancellationToken) =>
{
    var query = new GetPlatformServicesQuery();
    var result = await queryDispatcher
        .SendAsync<GetPlatformServicesQuery, IEnumerable<PlatformReadResponse>>(
            new GetPlatformServicesQuery(), cancellationToken);

    return Results.Ok(result);

}).ProducesProblem(StatusCodes.Status500InternalServerError)
  .Produces<IEnumerable<PlatformReadResponse>>(StatusCodes.Status200OK); 

app.MapPost("/platform", async (
    ICommandDispatcher commandDispatcher,
    [FromBody] PlatformCreateRequest request, 
    CancellationToken cancellationToken) =>
{
    var command = new CreatePlatformCommand(
        request.Name,
        request.Publisher,
        request.Cost);
    
    var result = await commandDispatcher
        .SendAsync<CreatePlatformCommand, int>(
            command, cancellationToken);

    return Results.Created($"/api/platforms/{result}", new { Id = result });

}).ProducesProblem(StatusCodes.Status500InternalServerError)
  .Produces<int>(StatusCodes.Status201Created); 

app.UseHttpsRedirection();

app.Run();
