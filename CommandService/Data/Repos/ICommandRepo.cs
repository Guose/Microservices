using CommandService.Models;

namespace CommandService.Data.Repos
{
    public interface ICommandRepo
    {
        //Commands
        Task<IEnumerable<Command>> GetAllCommandsForPlatformAsync(int platformId, CancellationToken cancellationToken = default);

        Task<Command?> GetCommandById(int platformId, int commandId, CancellationToken cancellationToken = default);

        Task CreateCommandAsync(int platformId, Command command, CancellationToken cancellationToken = default);

        // Platforms
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken cancellationToken = default);

        Task CreatePlatformAsync(Platform platform, CancellationToken cancellationToken = default);

        Task<bool> PlatformExistsAsync(int platformId, CancellationToken cancellationToken = default);

        Task<bool> ExternalPlatformExistsAsync(int externalId, CancellationToken cancellationToken = default);
    }
}
