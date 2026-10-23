using PlatformService.Services.Abstractions;

namespace PlatformService.Services.Dispatchers
{
    public sealed class QueryDispatcher(IServiceProvider provider) : IQueryDispatcher
    {
        public Task<TResult> SendAsync<TQuery, TResult>(
            TQuery query,
            CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            var handler = provider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

            var behaviors = provider
                .GetServices<IPipelineBehavior<TQuery, TResult>>()
                .Reverse()
                .ToArray();

            RequestHandlerDelegate<TResult> next =
                ct => handler.HandleAsync(query, ct);

            foreach (var behavior in behaviors)
            {
                var current = next;
                next = ct => behavior.HandleAsync(query, current, ct);
            }

            return next(cancellationToken);
        }
    }
}