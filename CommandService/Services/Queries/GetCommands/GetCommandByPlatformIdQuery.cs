using CommandService.Contracts;
using Shared.Abstractions;

namespace CommandService.Services.Queries.GetCommands
{
    public sealed record GetCommandByPlatformIdQuery(int CommandId, int PlatformId) : IQuery<CommandReadDto>;
}
