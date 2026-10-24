using PlatformService.Shared;

namespace PlatformService.Services.Abstractions
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