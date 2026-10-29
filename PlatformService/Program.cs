using Microsoft.EntityFrameworkCore;
using PlatformService.Contracts;
using PlatformService.Data;
using PlatformService.Data.PrepDb;
using PlatformService.Data.Repos;
using PlatformService.Endpoints;
using PlatformService.Services.AsyncDataServices;
using PlatformService.Services.Commands;
using PlatformService.Services.Queries;
using PlatformService.Services.SyncDataServices.Gprc;
using PlatformService.Services.SyncDataServices.Http;
using Shared.Abstractions;
using Shared.Dispatchers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddScoped<IDispatcher, Dispatcher>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddGrpc();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

await app.Services
    .GetRequiredService<IMessageBusClient>()
    .InitializeAsync();

app.MapPlatformEndpoints();

app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

DbSeeder.Initialize(app, builder.Environment.IsProduction());

app.Run();
