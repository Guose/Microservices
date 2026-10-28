using CommandService.Contracts;
using Shared.Abstractions;

namespace CommandService.Services.Commands
{
    public sealed record CreateCommandForPlatformCommand(int PlatformId, CommandCreateDto Command) : ICommand<CommandReadDto>;
}
