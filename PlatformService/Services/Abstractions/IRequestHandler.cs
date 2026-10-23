namespace PlatformService.Services.Abstractions
{
    public interface IRequestHandler<TRequest, TResult>
        where TRequest : IRequest<TResult>
    {
        Task<TResult> HandleAsync(
            TRequest request, 
            CancellationToken cancellationToken);
    }
}