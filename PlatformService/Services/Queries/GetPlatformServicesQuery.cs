using PlatformService.Contracts;
using Shared.Abstractions;

namespace PlatformService.Services.Queries
{
    public sealed record GetPlatformServicesQuery() : IQuery<IEnumerable<PlatformReadResponse>>;
}