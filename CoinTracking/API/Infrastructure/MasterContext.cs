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
        public DbSet<TagEntity> Tags { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterContext).Assembly);
        }
    }
}