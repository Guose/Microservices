using PlatformService.Contracts;
using PlatformService.Services.Abstractions;

namespace PlatformService.Services.Queries
{
    public sealed record GetPlatformServicesQuery() : IQuery<IEnumerable<PlatformReadResponse>>;
}