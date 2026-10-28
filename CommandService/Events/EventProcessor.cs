using CommandService.Contracts;
using CommandService.Data.Repos;
using CommandService.Models;
using Shared.Models;
using Shared.Results;
using System.Text.Json;

namespace CommandService.Events
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<Result> ProcessEventAsync(string message)
        {
            var eventType = DetermineEvent(message);
            return eventType switch
            {
                EventType.PlatformPublished => await AddPlatformAsync(message, CancellationToken.None),
                _ => Result.Failure(Error.Invalid("Event type not recognized"))
            };
        }

        private async Task<Result> AddPlatformAsync(string message, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            PlatformPublishedDto? dto;

            try
            {
                dto = JsonSerializer.Deserialize<PlatformPublishedDto>(message);
            }
            catch (JsonException)
            {
                return Result.Failure(Error.Invalid("Invalid PlatformPublished payload"));
            }

            if (dto is null)
                return Result.Failure(Error.Invalid("PlatformPublished payload is null"));

            try
            {
                var platform = new Platform
                {
                    ExternalId = dto.Id,
                    Name = dto.Name
                };

                if (await repo.ExternalPlatformExistsAsync(platform.ExternalId, cancellationToken))
                {
                    return Result.Success("Platform already exists in the database");
                }

                await repo.CreatePlatformAsync(platform, cancellationToken);
                await repo.SaveChangesAsync(cancellationToken);
                Console.WriteLine("--> Platform added to the database");
                return Result.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Failed to persist platform: {ex}");
                return Result.Failure(Error.Invalid($"Failed to persist platform: {ex}"));
            }
        }

        private static EventType DetermineEvent(string eventMessage)
        {
            Console.WriteLine("--> Determining Event");

            try
            {
                var eventDto = JsonSerializer.Deserialize<GenericEventDto>(eventMessage);

                return eventDto?.Event switch
                {
                    "Platform_Published" => EventType.PlatformPublished,
                    _ => EventType.Undetermined
                };
            }
            catch (JsonException)
            {
                return EventType.Undetermined;
            }
        }
    }

    public enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
