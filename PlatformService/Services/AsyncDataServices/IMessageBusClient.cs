using Shared.Models;

namespace PlatformService.Services.AsyncDataServices
{
    public interface IMessageBusClient
    {
        Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto);

        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
