using AutoMapper;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;

namespace Backend_portafolio.Sevices
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Post, PostViewModel>();

            CreateMap<MediaForm, Media>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => int.Parse(src.id)))
                .ForMember(dest => dest.mediatype_id, opt => opt.MapFrom(src => int.Parse(src.mediatype_id)));

            CreateMap<LinkForm, Link>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => int.Parse(src.id)))
                .ForMember(dest => dest.source_id, opt => opt.MapFrom(src => int.Parse(src.source_id)));

            CreateMap<UserViewModel, User>()
            .ForMember(dest => dest.emailNormalizado, opt => opt.MapFrom(src => src.email.ToUpper()));

            CreateMap<User, UserViewModel>();

            CreateMap<SocialNetworkViewModel, SocialNetwork>();

            CreateMap<BioViewModel, Bio>();
            CreateMap<User, UserApiViewModel>();
        }
    }
}
