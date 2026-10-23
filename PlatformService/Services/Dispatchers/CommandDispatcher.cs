using PlatformService.Services.Abstractions;

namespace PlatformService.Services.Dispatchers
{
    public sealed class CommandDispatcher(IServiceProvider provider) : ICommandDispatcher
    {
        public Task<TResult> SendAsync<TCommand, TResult>(
            TCommand command, 
            CancellationToken cancellationToken = default) 
            where TCommand : ICommand<TResult>
        {
            var handler = provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            
            var behaviors = provider
                .GetServices<IPipelineBehavior<TCommand, TResult>>()
                .Reverse()
                .ToArray();

            RequestHandlerDelegate<TResult> next = ct => handler.HandleAsync(command, ct);

            foreach (var behavior in behaviors)
            {
                var current = next;
                next = ct => behavior.HandleAsync(command, current, ct);
            }

            return next(cancellationToken);
        }
    }
}