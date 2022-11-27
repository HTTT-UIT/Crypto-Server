namespace API.Infrastructure.Entities
{
    public class TagEntity
    {
        public TagEntity()
        {
            Blogs = new List<BlogEntity>();
        }

        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public virtual ICollection<BlogEntity> Blogs { get; set; }
    }
}
