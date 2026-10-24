using PlatformService.Shared;

namespace PlatformService.Services.Abstractions;

public interface IDispatcher
{
    Task<Result<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}