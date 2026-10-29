using CommandService.Models;

namespace CommandService.Services.SyncDataServices
{
    public interface IPlatformDataClient
    {
        Task<IEnumerable<Platform>> ReturnAllPlatformsAsync();
    }
}
