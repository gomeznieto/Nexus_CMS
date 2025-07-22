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

            CreateMap<RoleViewModel, Role>();

            CreateMap<CategoryViewModel, Categoria>();

            CreateMap<FormatViewModel, Format>();

            CreateMap<PostViewModel, Post>();

            CreateMap<MediaTypeViewModel, MediaType>();

            CreateMap<SourceViewModel, Source>();

            CreateMap<HomeSectionModel, HomeSection>();


            //****************************************************
            //**************** ENTITY TO VIEWMODEL ***************
            //****************************************************

            CreateMap<User, UserViewModel>();

            CreateMap<Post, PostViewModel>();

            CreateMap<Role, RoleViewModel>();

            CreateMap<Categoria, CategoryViewModel>();

            CreateMap<Format, FormatViewModel>();

            CreateMap<MediaType, MediaTypeViewModel>();

            CreateMap<SocialNetwork, SocialNetworkViewModel>();

            CreateMap<Bio, BioViewModel>();

            CreateMap<Source, SourceViewModel>();

            CreateMap<HomeSection, HomeSectionModel>();


            //****************************************************
            //************************ API ***********************
            //****************************************************

            CreateMap<UserViewModel, ApiUserViewModel>();
            CreateMap<PostViewModel, ApiPostViewModel>();
            CreateMap<Media, ApiMediaViewModel>();
            CreateMap<Link, ApiLinkViewModel>();
            CreateMap<CategoryViewModel, ApiCategoryViewModel>();
        }
    }
}
