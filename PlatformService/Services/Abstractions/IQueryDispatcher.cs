namespace PlatformService.Services.Abstractions
{
    public interface IQueryDispatcher
    {
        Task<TResult> SendAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>;
    }
}