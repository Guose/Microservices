using PlatformService.Contracts;
using PlatformService.Data.Repos;
using PlatformService.Services.Abstractions;
using PlatformService.Shared;

namespace PlatformService.Services.Queries
{
    public class GetPlatformServicesQueryHandler : IQueryHandler<GetPlatformServicesQuery, IEnumerable<PlatformReadResponse>>
    {
        private readonly IPlatformRepo _platformRepo;

        public GetPlatformServicesQueryHandler(IPlatformRepo platformRepo)
        {
            _platformRepo = platformRepo;
        }

        public async Task<Result<IEnumerable<PlatformReadResponse>>> HandleAsync(GetPlatformServicesQuery query, CancellationToken cancellationToken)
        {
            var platforms = await _platformRepo.GetAllPlatformsAsync();

            if (platforms is null)
                return Result.Failure<IEnumerable<PlatformReadResponse>>(
                    new Error(
                        "Platforms.Empty",
                        "There are no platforms available."));

            var platformResponse = platforms.Select(p => new PlatformReadResponse
            (
                Id: p.Id,
                Name: p.Name,
                Publisher: p.Publisher,
                Cost: p.Cost
            )).ToList();


            return platformResponse;
        }
    }
}