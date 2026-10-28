using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data.Repos
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateCommandAsync(int platformId, Command command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var platform = await _context.Platforms
                .SingleOrDefaultAsync(
                    p => p.ExternalId == platformId,
                    cancellationToken);

            if (platform is null)
                throw new KeyNotFoundException($"Platform {platformId} not found.");

            command.PlatformId = platform.Id;
            await _context.Commands.AddAsync(command, cancellationToken);
        }

        public async Task CreatePlatformAsync(Platform platform, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(platform);

            await _context.Platforms.AddAsync(platform, cancellationToken);
        }

        public async Task<bool> ExternalPlatformExistsAsync(int externalId, CancellationToken cancellationToken = default)
        {
            return await _context.Platforms.AnyAsync(p => p.ExternalId == externalId, cancellationToken);
        }

        public async Task<IEnumerable<Command>> GetAllCommandsForPlatformAsync(int platformId, CancellationToken cancellationToken = default)
        {
            return await _context.Commands
                .AsNoTracking()
                .Where(c => c.Platform!.ExternalId == platformId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Platforms.ToListAsync(cancellationToken);
        }

        public async Task<Command?> GetCommandById(int platformId, int commandId, CancellationToken cancellationToken = default)
        {
            return await _context.Commands
                .Where(c => c.Platform!.ExternalId == platformId && c.Id == commandId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> PlatformExistsAsync(int platformId, CancellationToken cancellationToken = default)
        {
            return await _context.Platforms.AnyAsync(p => p.ExternalId == platformId, cancellationToken);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
