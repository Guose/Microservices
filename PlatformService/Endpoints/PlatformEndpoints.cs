using Microsoft.AspNetCore.Mvc;
using PlatformService.Contracts;
using PlatformService.Services.Commands;
using PlatformService.Services.Queries;
using Shared.Abstractions;

namespace PlatformService.Endpoints
{
    public static class PlatformEndpoints
    {
        private const string BaseRoute = "api/platform-service";

        public static void MapPlatformEndpoints(this WebApplication app)
        {
            app.MapGet($"{BaseRoute}", async (
                IDispatcher queryDispatcher,
                CancellationToken cancellationToken) =>
            {
                var query = new GetPlatformServicesQuery();
                var result = await queryDispatcher.SendAsync(query, cancellationToken);

                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Value);

            }).ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<IEnumerable<PlatformReadResponse>>(StatusCodes.Status200OK);

            app.MapPost($"{BaseRoute}", async (
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
        }
    }
}
