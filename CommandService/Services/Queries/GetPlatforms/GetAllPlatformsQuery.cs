using CommandService.Contracts;
using Shared.Abstractions;

namespace CommandService.Services.Queries.GetPlatforms
{
    public sealed record GetAllPlatformsQuery() : IQuery<List<PlatformReadDto>>;
}
