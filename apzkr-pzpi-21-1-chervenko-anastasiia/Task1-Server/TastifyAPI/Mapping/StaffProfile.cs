using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {
            CreateMap<Staff, StaffDto>();
            CreateMap<StaffDto, Staff>();
            CreateMap<StaffRegistrationDto, Staff>();
            CreateMap<Staff, StaffRegistrationDto>();
            
        }
    }
}