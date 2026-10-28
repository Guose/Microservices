using CommandService.Contracts;
using CommandService.Data.Repos;
using Shared.Abstractions;
using Shared.Results;

namespace CommandService.Services.Queries.GetCommands
{
    public sealed class GetCommandsForPlatformQueryHandler : IQueryHandler<GetCommandsForPlatformQuery, List<CommandReadDto>>
    {
        private readonly ICommandRepo _commandRepo;

        public GetCommandsForPlatformQueryHandler(ICommandRepo commandRepo)
        {
            _commandRepo = commandRepo;
        }

        public async Task<Result<List<CommandReadDto>>> HandleAsync(GetCommandsForPlatformQuery request, CancellationToken cancellationToken)
        {
            if (!await _commandRepo.PlatformExistsAsync(request.PlatformId, cancellationToken))
            {
                return Result.Failure<List<CommandReadDto>>(Error.NotFound($"Platform with ID {request.PlatformId} does not exist."));
            }

            var commands = await _commandRepo.GetAllCommandsForPlatformAsync(request.PlatformId, cancellationToken);

            if (commands is null || !commands.Any())
            {
                return Result.Failure<List<CommandReadDto>>(Error.NotFound($"No commands found for platform with ID {request.PlatformId}."));
            }

            var result = commands.Select(c => new CommandReadDto
            (
                c.Id,
                c.HowTo,
                c.CommandLine,
                request.PlatformId

            )).ToList();

            return Result<List<CommandReadDto>>.Success(result);
        }
    }
}
