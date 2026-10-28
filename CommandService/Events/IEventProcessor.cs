using Shared.Results;

namespace CommandService.Events
{
    public interface IEventProcessor
    {
        Task<Result> ProcessEventAsync(string message);
    }
}
