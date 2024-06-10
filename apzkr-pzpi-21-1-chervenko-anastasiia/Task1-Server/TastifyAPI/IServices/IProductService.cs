using System.Collections.Generic;
using System.Threading.Tasks;
using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IProductService
    {
        Task<List<Product>> GetAsync();
        Task<Product?> GetByIdAsync(string id);
        Task CreateAsync(Product newProduct);
        Task UpdateAsync(string id, Product updatedProduct);
        Task RemoveAsync(string id);
    }
}
