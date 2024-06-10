using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TastifyAPI.Services
{
    
    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductService(IMongoDatabase database)
        {
            _productCollection = database.GetCollection<Product>("Products");
        }

        public async Task<List<Product>> GetAsync() =>
            await _productCollection.Find(_ => true).ToListAsync();

        public async Task<Product?> GetByIdAsync(string id) =>
            await _productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Product newProduct) =>
            await _productCollection.InsertOneAsync(newProduct);

        public async Task UpdateAsync(string id, Product updatedProduct) =>
            await _productCollection.ReplaceOneAsync(x => x.Id == id, updatedProduct);

        public async Task RemoveAsync(string id) =>
            await _productCollection.DeleteOneAsync(x => x.Id == id);
    }
}
