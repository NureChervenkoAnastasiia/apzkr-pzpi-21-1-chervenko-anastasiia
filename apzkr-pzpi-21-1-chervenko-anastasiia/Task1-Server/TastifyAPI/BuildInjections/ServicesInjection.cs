using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.Services;

namespace TastifyAPI.BuildInjections
{
    internal static class ServicesInjection
    {
        internal static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<BookingService>();
            services.AddScoped<GuestService>();
            services.AddScoped<MenuService>();
            services.AddScoped<OrderService>();
            services.AddScoped<ProductService>();
            services.AddScoped<RestaurantService>();
            services.AddScoped<ScheduleService>();
            services.AddScoped<StaffService>();
            services.AddScoped<TableService>();
            services.AddScoped<IPasswordHasher<Guest>, PasswordHasher<Guest>>();
            services.AddScoped<IPasswordHasher<Staff>, PasswordHasher<Staff>>();

        }
    }
}
