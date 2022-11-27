namespace API.Infrastructure.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? LastUpdatedAt { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}
