using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IMenuService
    {
        Task<List<Menu>> GetAsync();
        Task<Menu?> GetByIdAsync(string id);
        Task CreateAsync(Menu newMenu);
        Task UpdateAsync(string id, Menu updatedMenu);
        Task RemoveAsync(string id);
        Task<List<Menu>> GetFirstDishesForRestaurantAsync(string restaurantId);
        Task<List<Menu>> GetSecondDishesForRestaurantAsync(string restaurantId);
        Task<List<Menu>> GetDrinksForRestaurantAsync(string restaurantId);
        Task<List<Menu>> GetRestaurantMenuAsync(string restaurantId);
        Task<List<DishPopularityDto>> GetMostPopularDishesAsync(string restaurantId);

    }
}
