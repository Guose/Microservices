using CommandService.Contracts;
using CommandService.Services.Commands;
using CommandService.Services.Queries.GetCommands;
using CommandService.Services.Queries.GetPlatforms;
using Shared.Abstractions;

namespace CommandService.Endpoints
{
    public static class ServiceEndpoints
    {
        private const string BasePath = "/api/com";
        private const string PlatformBaseRoute = BasePath + "/platforms";
        private const string CommandBaseRoute = PlatformBaseRoute + "/{platformId}/commands";

        public static void MapCommandEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet($"{CommandBaseRoute}", async (
                int platformId,
                IDispatcher queryDispatcher,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCommandsForPlatformQuery(platformId);
                var result = await queryDispatcher.SendAsync(query, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error?.Message);

            }).ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<List<CommandReadDto>>(StatusCodes.Status200OK);

            app.MapGet($"{CommandBaseRoute}/{{commandId}}", async (
                int platformId,
                int commandId,
                IDispatcher queryDispatcher,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCommandByPlatformIdQuery(platformId, commandId);
                var result = await queryDispatcher.SendAsync(query, cancellationToken);
                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error?.Message);
            }).WithName("GetCommandByPlatformId")
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<CommandReadDto>(StatusCodes.Status200OK);

            app.MapPost($"{CommandBaseRoute}", async (
                int platformId,
                CommandCreateDto commandCreateDto,
                IDispatcher commandDispatcher,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCommandForPlatformCommand(platformId, commandCreateDto);
                var result = await commandDispatcher.SendAsync(command, cancellationToken);

                return result.IsSuccess
                    ? Results.CreatedAtRoute(
                        routeName: "GetCommandByPlatformId",
                        routeValues: new { platformId, commandId = result.Value.Id },
                        value: result.Value)
                    : Results.BadRequest(result.Error?.Message);

            }).ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<CommandReadDto>(StatusCodes.Status201Created);
        }

        public static void MapPlatformEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost($"{BasePath}/ping", async (
                    CancellationToken cancellationToken) =>
            {
                Console.WriteLine("CommandService reached...");
                return Results.Ok("Pong");

            }).ProducesProblem(StatusCodes.Status500InternalServerError)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status200OK);

            app.MapGet($"{PlatformBaseRoute}", async (
                IDispatcher queryDispatcher,
                CancellationToken cancellationToken) =>
            {
                var query = new GetAllPlatformsQuery();
                var result = await queryDispatcher.SendAsync(query, cancellationToken);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error?.Message);

            }).ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<List<PlatformReadDto>>(StatusCodes.Status200OK);
        }
    }
}
