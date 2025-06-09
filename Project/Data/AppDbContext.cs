using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Baby> Babies { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Vaccination> Vaccinations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Measurement>()
                 .HasOne(m => m.Baby)
                 .WithMany(b => b.Measurements)
                 .HasForeignKey(m => m.BabyId);
            modelBuilder.Entity<Baby>()
                .HasMany(b => b.Vaccinations)
                .WithOne(v => v.Baby)
                .HasForeignKey(v => v.BabyId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
