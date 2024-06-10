using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace TastifyAPI.Services
{

    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orderCollection;

        public OrderService(IMongoDatabase database)
        {
            _orderCollection = database.GetCollection<Order>("Orders");
        }
        public async Task<List<Order>> GetAsync()
        {
            var orders = await _orderCollection.Find(_ => true).ToListAsync();

            foreach (var order in orders)
            {
                order.OrderDateTime = order.OrderDateTime.ToLocalTime();
            }

            return orders;
        }

        public async Task<Order?> GetByIdAsync(string id)
        {
            var order = await _orderCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            order.OrderDateTime = order.OrderDateTime.ToLocalTime();

            return order;
        }

        public async Task CreateAsync(Order newOrder)
        {
            newOrder.OrderDateTime = newOrder.OrderDateTime.ToUniversalTime();

            await _orderCollection.InsertOneAsync(newOrder);
        }

        public async Task UpdateAsync(string id, Order updatedOrder)
        {
            updatedOrder.OrderDateTime = updatedOrder.OrderDateTime.ToUniversalTime();

            await _orderCollection.ReplaceOneAsync(x => x.Id == id, updatedOrder);
        }

        public async Task RemoveAsync(string id) =>
            await _orderCollection.DeleteOneAsync(x => x.Id == id);

    }
}
