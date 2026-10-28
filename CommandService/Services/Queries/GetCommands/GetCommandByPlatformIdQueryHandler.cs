using CommandService.Contracts;
using CommandService.Data.Repos;
using Shared.Abstractions;
using Shared.Results;

namespace CommandService.Services.Queries.GetCommands
{
    public class GetCommandByPlatformIdQueryHandler : IQueryHandler<GetCommandByPlatformIdQuery, CommandReadDto>
    {
        private readonly ICommandRepo _commandRepo;

        public GetCommandByPlatformIdQueryHandler(ICommandRepo commandRepo)
        {
            _commandRepo = commandRepo;
        }

        public async Task<Result<CommandReadDto>> HandleAsync(GetCommandByPlatformIdQuery request, CancellationToken cancellationToken)
        {
            var command = await _commandRepo.GetCommandById(request.PlatformId, request.CommandId, cancellationToken);

            if (command is null)
            {
                return Result.Failure<CommandReadDto>(Error.NotFound("Command not found"));
            }

            return Result.Success(new CommandReadDto
            (
                command.Id,
                command.HowTo,
                command.CommandLine,
                request.PlatformId
            ));
        }
    }
}
