using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IOrderService
    {
        Task<List<Order>> GetAsync();
        Task<Order?> GetByIdAsync(string id);
        Task CreateAsync(Order newOrder);
        Task UpdateAsync(string id, Order updatedOrder);
        Task RemoveAsync(string id);
    }
}
