using PlatformService.Models;

namespace PlatformService.Data.Repos
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChangesAsync();

        Task<IEnumerable<Platform>> GetAllPlatformsAsync();

        Task<Platform?> GetPlatformByIdAsync(int id);

        Task<int> CreatePlatformAsync(Platform platform);

        // Task<bool> UpdatePlatformAsync(Platform platform);

        // Task<bool> DeletePlatformAsync(int id);
    }
}