using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions;
using Shared.Results;
using System.Reflection;

namespace Shared.Dispatchers
{
    public sealed class Dispatcher(IServiceProvider provider) : IDispatcher
    {
        public Task<Result<TResponse>> SendAsync<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            var method = typeof(Dispatcher)
                .GetMethod(
                    nameof(DispatchCoreAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(request.GetType(), typeof(TResponse));

            return (Task<Result<TResponse>>)method.Invoke(this, [request, cancellationToken])!;
        }

        private Task<Result<TResponse>> DispatchCoreAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken)
            where TRequest : IRequest<TResponse>
        {
            var handler = provider.GetService<IRequestHandler<TRequest, TResponse>>();

            var behaviors = provider
                .GetServices<IPipelineBehavior<TRequest, Result<TResponse>>>()
                .Reverse()
                .ToArray();

            RequestHandlerDelegate<Result<TResponse>> next = ct => handler!.HandleAsync(request, ct);

            foreach (var behavior in behaviors)
            {
                var current = next;
                next = ct => behavior.HandleAsync(request, current, ct);
            }

            return next(cancellationToken);
        }
    }
}
