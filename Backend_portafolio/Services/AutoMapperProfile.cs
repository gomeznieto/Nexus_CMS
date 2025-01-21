using AutoMapper;
using Backend_portafolio.Entities;
using Backend_portafolio.Models;

namespace Backend_portafolio.Sevices
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //****************************************************
            //**************** VIEWMODEL TO ENTITY ***************
            //****************************************************

            CreateMap<MediaForm, Media>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => int.Parse(src.id)))
                .ForMember(dest => dest.mediatype_id, opt => opt.MapFrom(src => int.Parse(src.mediatype_id)));

            CreateMap<LinkForm, Link>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => int.Parse(src.id)))
                .ForMember(dest => dest.source_id, opt => opt.MapFrom(src => int.Parse(src.source_id)));

            CreateMap<UserViewModel, User>()
            .ForMember(dest => dest.emailNormalizado, opt => opt.MapFrom(src => src.email.ToUpper()));

            CreateMap<BioViewModel, Bio>();

            CreateMap<SocialNetworkViewModel, SocialNetwork>();


            //****************************************************
            //**************** ENTITY TO VIEWMODEL ***************
            //****************************************************

            CreateMap<User, UserViewModel>();

            CreateMap<Post, PostViewModel>();


            //****************************************************
            //************************ API ***********************
            //****************************************************

            CreateMap<User, ApiUserViewModel>();
            CreateMap<Post, ApiPostViewModel>();
            CreateMap<Media, ApiMediaViewModel>();
            CreateMap<Link, ApiLinkViewModel>();
            CreateMap<Categoria, ApiCategoryViewModel>();
        }
    }
}
