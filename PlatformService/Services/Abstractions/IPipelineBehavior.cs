namespace PlatformService.Services.Abstractions
{
    public delegate Task<TResult> RequestHandlerDelegate<TResult>(CancellationToken cancellationToken);
    public interface IPipelineBehavior<TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken);
    }
}