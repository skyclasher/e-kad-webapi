using AutoMapper;
using ECard.Entities.Entities;
using WebApi.Dtos;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<ECardDetail, ECardDetailDto>().ReverseMap();
            CreateMap<Rsvp, RsvpDto>().ReverseMap();
        }
    }
}