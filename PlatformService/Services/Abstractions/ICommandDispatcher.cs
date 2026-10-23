namespace PlatformService.Services.Abstractions
{
    public interface ICommandDispatcher
    {
        Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>;
    }
}