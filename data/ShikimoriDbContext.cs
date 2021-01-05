using Microsoft.EntityFrameworkCore;
using Shikimori.Data.Models;

namespace Shikimori.Data
{
    public class ShikimoriDbContext : DbContext
    {
        public ShikimoriDbContext(DbContextOptions<ShikimoriDbContext> options) : base(options)
        {
        }

        public DbSet<Video> Videos { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(x => x.Url);
                entity.Property(x => x.Url).HasMaxLength(1000);
                entity.Property(x => x.ImageUrl).HasMaxLength(1000);
                entity.Property(x => x.TitleRus).HasMaxLength(500);
                entity.Property(x => x.TitleEng).HasMaxLength(500);
                entity.Property(x => x.Description).HasColumnType("text");
                entity.Property(x => x.Genres).HasColumnType("jsonb");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(x => x.Key);
                entity.Property(x => x.Key).HasMaxLength(100);
                entity.Property(x => x.Value).HasMaxLength(500);
            });
        }
    }
}
