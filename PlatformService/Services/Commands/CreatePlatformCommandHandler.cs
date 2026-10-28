using PlatformService.Contracts;
using PlatformService.Data.Repos;
using PlatformService.Models;
using PlatformService.Services.AsyncDataServices;
using PlatformService.Services.SyncDataServices.Http;
using Shared.Abstractions;
using Shared.Models;
using Shared.Results;

namespace PlatformService.Services.Commands
{
    public class CreatePlatformCommandHandler : ICommandHandler<CreatePlatformCommand, PlatformReadResponse>
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public CreatePlatformCommandHandler(
            IPlatformRepo platformRepo,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _platformRepo = platformRepo;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }
        public async Task<Result<PlatformReadResponse>> HandleAsync(CreatePlatformCommand command, CancellationToken cancellationToken = default)
        {
            var platform = new Platform
            {
                Name = command.Name,
                Publisher = command.Publisher,
                Cost = command.Cost
            };

            await _platformRepo.CreatePlatformAsync(platform);

            var response = new PlatformReadResponse(
                platform.Id,
                platform.Name,
                platform.Publisher,
                platform.Cost);

            try
            {
                await _commandDataClient.SendPlatformCommandAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
                return Result.Failure<PlatformReadResponse>
                    (Error.ServiceUnavailable("Failed to send platform data to command service."));
            }

            try
            {
                var publishMessage = new PlatformPublishedDto
                (
                    platform.Id,
                    platform.Name,
                    "Platform_Published"
                );

                await _messageBusClient.PublishNewPlatformAsync(publishMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
                return Result.Failure<PlatformReadResponse>
                    (Error.ServiceUnavailable("Failed to send platform message to message bus."));
            }

            if (platform.Id == 0)
                return Result.Failure<PlatformReadResponse>
                    (Error.ServiceUnavailable("Failed to create platform."));

            return Result.Success(response);
        }
    }
}