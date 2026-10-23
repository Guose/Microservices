using PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data.Repos
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;
        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreatePlatformAsync(Platform platform)
        {
            ArgumentNullException.ThrowIfNull(platform);

            var platformEntry = await _context.Platforms.AddAsync(platform);
            
            if(!await SaveChangesAsync())
                throw new Exception("Failed to create platform.");
            
            return platformEntry.Entity.Id;
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
        {
            return await _context.Set<Platform>().ToListAsync();
        }

        public async Task<Platform?> GetPlatformByIdAsync(int id)
        {
            return await _context.Set<Platform>().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}