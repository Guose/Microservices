using PlatformService.Services.Abstractions;

namespace PlatformService.Services.Commands
{
    public sealed record CreatePlatformCommand(string Name, string Publisher, string Cost) : ICommand<int>;
}