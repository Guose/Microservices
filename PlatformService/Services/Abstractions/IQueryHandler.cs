namespace PlatformService.Services.Abstractions
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
    }
}