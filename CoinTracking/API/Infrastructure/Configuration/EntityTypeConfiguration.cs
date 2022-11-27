using API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Configuration
{
    public class BlogEntityTypeConfiguration : IEntityTypeConfiguration<BlogEntity>
    {
        public void Configure(EntityTypeBuilder<BlogEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(x => x.Author)
                .WithMany(o => o.Blogs)
                .HasForeignKey(x => x.AuthorId);
        }
    }

    public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();
        }
    }

    public class ViewedEntityTypeConfiguration : IEntityTypeConfiguration<ViewedEntity>
    {
        public void Configure(EntityTypeBuilder<ViewedEntity> builder)
        {
            builder.HasKey(x => new { x.CoinId, x.UserId });
            builder.HasOne(x => x.CoinViewed)
                .WithMany(o => o.ViewedUsers)
                .HasForeignKey(x => x.CoinId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.UserViewed)
                .WithMany(o => o.ViewedCoin)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}