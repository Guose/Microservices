using Shared.Results;

namespace Shared.Abstractions;

public interface IDispatcher
{
    Task<Result<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}