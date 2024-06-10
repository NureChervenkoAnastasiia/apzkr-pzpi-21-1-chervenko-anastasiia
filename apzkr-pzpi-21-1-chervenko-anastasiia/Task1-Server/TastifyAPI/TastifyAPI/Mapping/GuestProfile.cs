using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class GuestProfile : Profile
    {
        public GuestProfile()
        {
            CreateMap<Guest, GuestDto>();
            CreateMap<GuestDto, Guest>();
            CreateMap<Guest, GuestRegistrationDto>();
            CreateMap<GuestRegistrationDto, Guest>();
        }
    }
}
