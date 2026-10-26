using PlatformService.Contracts;
using PlatformService.Data.Repos;
using PlatformService.Models;
using PlatformService.Services.Abstractions;
using PlatformService.Shared;
using PlatformService.SyncDataServices.Http;
namespace PlatformService.Services.Commands
{
    public class CreatePlatformCommandHandler : ICommandHandler<CreatePlatformCommand, PlatformReadResponse>
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly ICommandDataClient _commandDataClient;

        public CreatePlatformCommandHandler(IPlatformRepo platformRepo, ICommandDataClient commandDataClient)
        {
            _platformRepo = platformRepo;
            _commandDataClient = commandDataClient;
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

            if (platform.Id == 0)
                throw new Exception("Failed to create platform.");

            return Result.Success(response);
        }
    }
}