using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TastifyAPI.Entities;

namespace TastifyAPI.BuildInjections
{
    public static class MongoCollectionExtensions
    {
        public static void AddMongoCollections(this IServiceCollection services)
        {
            services.AddSingleton<IMongoCollection<Booking>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Booking>("Booking");
            });

            services.AddSingleton<IMongoCollection<Guest>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Guest>("Guest");
            });

            services.AddSingleton<IMongoCollection<Menu>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Menu>("Menu");
            });

            services.AddSingleton<IMongoCollection<Order>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Order>("Order");
            });

            services.AddSingleton<IMongoCollection<Product>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Product>("Product");
            });

            services.AddSingleton<IMongoCollection<Restaurant>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Restaurant>("Restaurant");
            });

            services.AddSingleton<IMongoCollection<Schedule>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Schedule>("Schedule");
            });

            services.AddSingleton<IMongoCollection<Staff>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Staff>("Staff");
            });

            services.AddSingleton<IMongoCollection<Table>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database.GetCollection<Table>("Table");
            });
        }
    }
}
