using API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Configuration
{
    public interface IBlogEntityTypeConfiguration
    {
        void Configure(EntityTypeBuilder<BlogEntity> builder);
    }
}