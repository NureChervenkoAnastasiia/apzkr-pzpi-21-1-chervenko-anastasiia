using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TastifyAPI.Mapping;
using Microsoft.AspNetCore.Hosting;

namespace TastifyAPI.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(
                Assembly.GetAssembly(typeof(RestaurantProfile)), 
                Assembly.GetAssembly(typeof(MenuProfile)), 
                Assembly.GetAssembly(typeof(BookingProfile)),
                Assembly.GetAssembly(typeof(ProductProfile)));

            return services;
        }
    }
}
