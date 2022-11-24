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
                .ForMember(d => d.Content, opt => opt.Condition(s => !string.IsNullOrEmpty(s.Content)));
        }
    }
}
