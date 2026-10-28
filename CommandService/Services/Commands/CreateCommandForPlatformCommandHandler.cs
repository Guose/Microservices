using CommandService.Contracts;
using CommandService.Data.Repos;
using CommandService.Models;
using Shared.Abstractions;
using Shared.Results;

namespace CommandService.Services.Commands
{
    public class CreateCommandForPlatformCommandHandler : ICommandHandler<CreateCommandForPlatformCommand, CommandReadDto>
    {
        private readonly ICommandRepo _commandRepo;

        public CreateCommandForPlatformCommandHandler(ICommandRepo commandRepo)
        {
            _commandRepo = commandRepo;
        }

        public async Task<Result<CommandReadDto>> HandleAsync(CreateCommandForPlatformCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if (!await _commandRepo.PlatformExistsAsync(request.PlatformId))
            {
                return Result.Failure<CommandReadDto>(Error.NotFound("Platform cannot be found"));
            }

            var command = new Command
            {
                HowTo = request.Command.HowTo,
                CommandLine = request.Command.CommandLine
            };

            await _commandRepo.CreateCommandAsync(request.PlatformId, command, cancellationToken);

            if (!await _commandRepo.SaveChangesAsync(cancellationToken))
            {
                return Result.Failure<CommandReadDto>(Error.Conflict("Failed to save the command."));
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
