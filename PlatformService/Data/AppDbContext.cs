using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> contextOptions) : base(contextOptions) { }
        public DbSet<Platform> Platforms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Platform>().HasKey(p => p.Id);

            modelBuilder.Entity<Platform>().Property(p => p.Name).IsRequired();

            modelBuilder.Entity<Platform>().Property(p => p.Publisher).IsRequired();

            modelBuilder.Entity<Platform>().Property(p => p.Cost).IsRequired();
        }
    }
}