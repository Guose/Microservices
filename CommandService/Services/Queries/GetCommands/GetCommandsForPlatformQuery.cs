using CommandService.Contracts;
using Shared.Abstractions;

namespace CommandService.Services.Queries.GetCommands
{
    public sealed record GetCommandsForPlatformQuery(int PlatformId) : IQuery<List<CommandReadDto>>;
}
