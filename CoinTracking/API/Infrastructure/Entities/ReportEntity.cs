using API.Infrastructure.Entities.Common;

namespace API.Infrastructure.Entities
{
    public class ReportEntity : BaseEntity
    {
        public int Id { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;  

        public virtual UserEntity UserReport { get; set; } = new();

        public virtual BlogEntity BlogReport { get; set; } = new();
    }
}
