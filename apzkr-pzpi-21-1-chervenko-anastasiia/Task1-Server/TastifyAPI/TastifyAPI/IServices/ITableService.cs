using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface ITableService
    {
        Task<List<Table>> GetAsync();
        Task<Table?> GetByIdAsync(string id);
        Task CreateAsync(Table newTable);
        Task UpdateAsync(string id, Table updatedTable);
        Task RemoveAsync(string id);
    }
}
