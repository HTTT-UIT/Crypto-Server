using API.Infrastructure.Entities;
using AutoMapper;

namespace API.Configurations
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateBlogMap();
            CreateCommentMap();
            CreateTagMap();
        }

        public void CreateBlogMap()
        {
            CreateMap<Features.Blogs.Commands.Update.Command, BlogEntity>()
                .ForMember(d => d.Header, opt => opt.Condition(s => !string.IsNullOrEmpty(s.Header)))
                .ForMember(d => d.Content, opt => opt.Condition(s => !string.IsNullOrEmpty(s.Content)))
                .ForMember(d => d.SubContent, opt => opt.Condition(s => !string.IsNullOrEmpty(s.SubContent)))
                .ForMember(d => d.AuthorId, opt => opt.Ignore());

            CreateMap<BlogEntity, Features.Blogs.Queries.Get.Response>()
                .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author == null ? string.Empty : s.Author.Name))
                .ForMember(d => d.AuthorId, opt => opt.MapFrom(s => s.Author == null ? Guid.Empty : s.Author.Id))
                .ForMember(d => d.AuthorImageUrl, opt => opt.MapFrom(s => s.Author == null ? string.Empty : s.Author.ProfileImageUrl))
                .ForMember(d => d.TotalFollower, opt => opt.MapFrom(s => s.FollowUsers.Count));

            CreateMap<BlogEntity, Features.Blogs.Queries.List.ResponseItem>()
                .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author == null ? string.Empty : s.Author.Name))
                .ForMember(d => d.AuthorId, opt => opt.MapFrom(s => s.Author == null ? Guid.Empty : s.Author.Id))
                .ForMember(d => d.AuthorImageUrl, opt => opt.MapFrom(s => s.Author == null ? string.Empty : s.Author.ProfileImageUrl))
                .ForMember(d => d.TotalFollower, opt => opt.MapFrom(s => s.FollowUsers.Count));
        }

        public void CreateCommentMap()
        {
            CreateMap<CommentEntity, Features.Blogs.Comment.Queries.ListComments.CommentViewModel>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.User.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User.Name ?? string.Empty))
                .ForMember(d => d.ProfileImageUrl, opt => opt.MapFrom(s => s.User.ProfileImageUrl ?? string.Empty));
        }

        public void CreateTagMap()
        {
            CreateMap<TagEntity, Features.Blogs.Queries.List.Tag>();
            CreateMap<TagEntity, Features.Blogs.Queries.Get.Tag>();
        }
    }
}