using API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure
{
    public class MasterContext : DbContext
    {
        public MasterContext(DbContextOptions<MasterContext> options)
            : base(options)
        {
        }

        public DbSet<BlogEntity> Blogs { get; set; } = default!;
        public DbSet<UserEntity> Users { get; set; } = default!;
        public DbSet<CoinEntity> Coins { get; set; } = default!;
        public DbSet<ViewedEntity> Views { get; set; } = default!;
        public DbSet<CommentEntity> Comments { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterContext).Assembly);

            modelBuilder.Entity<ViewedEntity>()
                .HasKey(x => new { x.CoinId, x.UserId });
            modelBuilder.Entity<ViewedEntity>()
                .HasOne(x => x.CoinViewed)
                .WithMany(o => o.ViewedUsers)
                .HasForeignKey(x => x.CoinId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ViewedEntity>()
                .HasOne(x => x.UserViewed)
                .WithMany(o => o.ViewedCoin)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}