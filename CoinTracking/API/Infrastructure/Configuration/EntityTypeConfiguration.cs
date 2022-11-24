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
}