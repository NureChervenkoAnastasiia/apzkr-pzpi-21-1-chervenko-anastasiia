using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class TableProfile : Profile
    {
        public TableProfile()
        {
            CreateMap<Table, TableDto>();
            CreateMap< TableDto, Table>();
        }
    }
}
