using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class MenuProfile : Profile
    {
        public MenuProfile() {
            CreateMap<Menu, MenuDto>();
            CreateMap<MenuDto, Menu>();
        }
    }
}
