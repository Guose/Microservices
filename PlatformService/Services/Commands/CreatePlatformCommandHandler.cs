using PlatformService.Data.Repos;
using PlatformService.Models;
using PlatformService.Services.Abstractions;
using PlatformService.Shared;
namespace PlatformService.Services.Commands
{
    public class CreatePlatformCommandHandler : ICommandHandler<CreatePlatformCommand, int>
    {
        private readonly IPlatformRepo _platformRepo;

        public CreatePlatformCommandHandler(IPlatformRepo platformRepo)
        {
            _platformRepo = platformRepo;
        }
        public async Task<Result<int>> HandleAsync(CreatePlatformCommand command, CancellationToken cancellationToken = default)
        {
            var platform = new Platform
            {
                Name = command.Name,
                Publisher = command.Publisher,
                Cost = command.Cost
            };

            await _platformRepo.CreatePlatformAsync(platform);

            if (platform.Id == 0)
                throw new Exception("Failed to create platform.");

            return Result.Success(platform.Id);
        }
    }
}