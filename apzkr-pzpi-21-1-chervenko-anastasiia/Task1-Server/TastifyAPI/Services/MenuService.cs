using MongoDB.Driver;
using TastifyAPI.DTOs;
using TastifyAPI.DTOs.Features_DTOs;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMongoCollection<Menu> _menuCollection;
        private readonly IMongoCollection<OrderItem> _orderItemCollection;

        public MenuService(IMongoDatabase database)
        {
            _menuCollection = database.GetCollection<Menu>("Menu");
            _orderItemCollection = database.GetCollection<OrderItem>("Order_items");
        }

        public async Task<List<Menu>> GetAsync() =>
            await _menuCollection.Find(_ => true).ToListAsync();

        public async Task<List<Menu>> GetRestaurantMenuAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.RestaurantId == restaurantId).ToListAsync();


        public async Task<Menu?> GetByIdAsync(string id) =>
            await _menuCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Menu newMenu) =>
            await _menuCollection.InsertOneAsync(newMenu);

        public async Task UpdateAsync(string id, Menu updatedMenu) =>
            await _menuCollection.ReplaceOneAsync(x => x.Id == id, updatedMenu);

        public async Task RemoveAsync(string id) =>
            await _menuCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Menu>> GetFirstDishesForRestaurantAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Type == "Перші страви" && x.RestaurantId == restaurantId).ToListAsync();

        public async Task<List<Menu>> GetSecondDishesForRestaurantAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Type == "Другі страви" && x.RestaurantId == restaurantId).ToListAsync();

        public async Task<List<Menu>> GetDrinksForRestaurantAsync(string restaurantId) =>
            await _menuCollection.Find(x => x.Type == "Напій" && x.RestaurantId == restaurantId).ToListAsync();

        public async Task<List<DishPopularityDto>> GetMostPopularDishesAsync(string restaurantId)
        {
            var result = new List<DishPopularityDto>();

            var restaurantDishes = await _menuCollection.Find(x => x.RestaurantId == restaurantId).ToListAsync();

            var orderItems = await _orderItemCollection.Find(x => restaurantDishes.Select(d => d.Id).Contains(x.MenuId)).ToListAsync();

            var groupedDishes = orderItems.GroupBy(x => x.MenuId)
                                           .Select(group => new
                                           {
                                               MenuId = group.Key,
                                               TotalAmount = group.Sum(x => x.Amount)
                                           });

            var uniqueMenuIds = groupedDishes.Select(x => x.MenuId).Distinct();

            foreach (var menuId in uniqueMenuIds)
            {
                var totalAmount = groupedDishes.Where(x => x.MenuId == menuId).Sum(x => x.TotalAmount);

                var menu = await _menuCollection.Find(x => x.Id == menuId).FirstOrDefaultAsync();
                if (menu != null)
                {
                    var dishPopularity = new DishPopularityDto
                    {
                        Name = menu.Name,
                        OrdersCount = totalAmount
                    };

                    result.Add(dishPopularity);
                }
            }

            result = result.OrderByDescending(x => x.OrdersCount).ThenByDescending(x => x.Name).ToList();

            return result;
        }
    }
}

