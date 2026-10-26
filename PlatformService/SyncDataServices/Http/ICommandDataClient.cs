using PlatformService.Contracts;

namespace PlatformService.SyncDataServices.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformCommandAsync(PlatformReadResponse platform);
    }
}
