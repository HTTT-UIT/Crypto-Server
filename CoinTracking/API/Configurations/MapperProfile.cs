using API.Infrastructure.Entities;
using AutoMapper;

namespace API.Configurations
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Features.Blogs.Commands.Update.Request, BlogEntity>()
                .ForMember(d => d.Header, opt => opt.Condition(s => !string.IsNullOrEmpty(s.Header)))
                .ForMember(d => d.Content, opt => opt.Condition(s => !string.IsNullOrEmpty(s.Content)))
                .ForMember(d => d.AuthorId, opt => opt.Ignore());

            CreateMap<CommentEntity, Features.Blogs.Comment.Queries.ListComments.CommentViewModel>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.User.Id))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.User.Name ?? string.Empty));

            CreateMap<BlogEntity, Features.Blogs.Queries.Get.Response>()
                .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author == null ? string.Empty : s.Author.Name))
                .ForMember(d => d.TotalFollower, opt => opt.MapFrom(s => s.FollowUsers.Count));

            CreateMap<BlogEntity, Features.Blogs.Queries.List.ResponseItem>()
                .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author == null ? string.Empty : s.Author.Name))
                .ForMember(d => d.TotalFollower, opt => opt.MapFrom(s => s.FollowUsers.Count));

            CreateMap<TagEntity, Features.Blogs.Queries.List.Tag>();
            CreateMap<TagEntity, Features.Blogs.Queries.Get.Tag>();
        }
    }
}