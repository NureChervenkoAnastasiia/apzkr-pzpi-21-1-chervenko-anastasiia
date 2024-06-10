using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public interface IRestaurantService
    {
        Task<List<Restaurant>> GetAsync();
        Task<Restaurant?> GetByIdAsync(string id);
        Task CreateAsync(Restaurant newRestaurant);
        Task UpdateAsync(string id, Restaurant updatedRestaurant);
        Task RemoveAsync(string id);
    }
}
