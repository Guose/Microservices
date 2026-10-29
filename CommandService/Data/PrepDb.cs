using CommandService.Data.Repos;
using CommandService.Models;
using CommandService.Services.SyncDataServices;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static async Task PrepPopulation(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

            var platforms = await grpcClient!.ReturnAllPlatformsAsync();

            await SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
        }

        private static async Task SeedData(ICommandRepo? commandRepo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new platforms...");

            foreach (var platform in platforms)
            {
                if (!await commandRepo!.PlatformExistsAsync(platform.ExternalId))
                {
                    await commandRepo.CreatePlatformAsync(platform);
                    Console.WriteLine($"--> Seeding new platform: {platform.Name}");
                }

                await commandRepo.SaveChangesAsync();
            }
        }
    }
}
