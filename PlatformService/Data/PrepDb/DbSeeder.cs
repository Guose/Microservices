using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data.PrepDb
{
    public static class DbSeeder
    {
        public static void Initialize(IApplicationBuilder app, bool isProd)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            Seed(context, isProd);
        }
        private static void Seed(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not apply migrations: {ex.Message}");
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding initial \'Platform\' data...");

                context.Platforms.AddRange(
                    new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Visual Studio", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                    new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Data already exists in the database.");
            }
        }
    }
}