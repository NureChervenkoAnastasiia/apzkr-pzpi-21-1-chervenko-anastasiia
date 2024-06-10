using AutoMapper;
using TastifyAPI.DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}



