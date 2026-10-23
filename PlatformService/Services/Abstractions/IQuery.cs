using PlatformService.Shared;

namespace PlatformService.Services.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
}