using AutoMapper;
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
        }
    }
}
