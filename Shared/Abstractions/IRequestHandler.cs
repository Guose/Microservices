using Shared.Results;

namespace Shared.Abstractions
{
    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<Result<TResponse>> HandleAsync(
            TRequest request,
            CancellationToken cancellationToken);
    }

    public interface IRequestHandler<TRequest>
        where TRequest : IRequest<Result>
    {
        Task<Result> HandleAsync(
            TRequest request,
            CancellationToken cancellationToken);
    }
}