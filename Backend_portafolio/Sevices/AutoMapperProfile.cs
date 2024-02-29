using AutoMapper;
using Backend_portafolio.Models;

namespace Backend_portafolio.Sevices
{
	public class AutoMapperProfile : Profile
	{
        public AutoMapperProfile()
        {
            CreateMap<Post, PostViewModel>();
        }
    }
}
