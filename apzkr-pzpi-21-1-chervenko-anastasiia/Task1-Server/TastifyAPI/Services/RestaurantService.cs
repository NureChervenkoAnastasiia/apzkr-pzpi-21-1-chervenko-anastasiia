using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Data;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IMongoCollection<Restaurant> _restaurantCollection;

        public RestaurantService(IMongoDatabase database)
        {
            _restaurantCollection = database.GetCollection<Restaurant>("Restaurants");
        }

        public async Task<List<Restaurant>> GetAsync() =>
            await _restaurantCollection.Find(_ => true).ToListAsync();

        public async Task<Restaurant?> GetByIdAsync(string id) =>
    await _restaurantCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Restaurant newRestaurant) =>
            await _restaurantCollection.InsertOneAsync(newRestaurant);

        public async Task UpdateAsync(string id, Restaurant updatedRestaurant) =>
            await _restaurantCollection.ReplaceOneAsync(x => x.Id == id, updatedRestaurant);

        public async Task RemoveAsync(string id) =>
            await _restaurantCollection.DeleteOneAsync(x => x.Id == id);
    }
}
