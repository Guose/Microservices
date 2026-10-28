using CommandService.Contracts;
using CommandService.Data.Repos;
using Shared.Abstractions;
using Shared.Results;

namespace CommandService.Services.Queries.GetPlatforms
{
    public class GetAllPlatformsQueryHandler : IQueryHandler<GetAllPlatformsQuery, List<PlatformReadDto>>
    {
        private readonly ICommandRepo _commandRepo;

        public GetAllPlatformsQueryHandler(ICommandRepo commandRepo)
        {
            _commandRepo = commandRepo;
        }

        public async Task<Result<List<PlatformReadDto>>> HandleAsync(GetAllPlatformsQuery request, CancellationToken cancellationToken)
        {
            var platforms = await _commandRepo.GetAllPlatformsAsync(cancellationToken);

            if (platforms is null || !platforms.Any())
            {
                return Result.Failure<List<PlatformReadDto>>(Error.NotFound("No platforms found."));
            }

            var result = platforms.Select(p => new PlatformReadDto
            (
                p.Id,
                p.Name

            )).ToList();

            return Result.Success(result);
        }
    }
}
