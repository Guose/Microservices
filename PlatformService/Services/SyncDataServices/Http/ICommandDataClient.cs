using PlatformService.Contracts;

namespace PlatformService.Services.SyncDataServices.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformCommandAsync(PlatformReadResponse platform);
    }
}
