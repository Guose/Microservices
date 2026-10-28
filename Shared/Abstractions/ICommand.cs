using Shared.Results;

namespace Shared.Abstractions
{
    public interface ICommand : IRequest<Result>, IBaseCommand
    {
    }

    public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand
    {
    }

    public interface IBaseCommand
    { }
}