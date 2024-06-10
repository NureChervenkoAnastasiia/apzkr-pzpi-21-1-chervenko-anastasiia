using MongoDB.Driver;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class TableService
    {
        private readonly IMongoCollection<Table> _tableCollection;

        public TableService(IMongoDatabase database)
        {
            _tableCollection = database.GetCollection<Table>("Tables");
        }

        public async Task<List<Table>> GetAsync() =>
            await _tableCollection.Find(_ => true).ToListAsync();

        public async Task<Table?> GetByIdAsync(string id) =>
            await _tableCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Table newTable) =>
            await _tableCollection.InsertOneAsync(newTable);

        public async Task UpdateAsync(string id, Table updatedTable) =>
            await _tableCollection.ReplaceOneAsync(x => x.Id == id, updatedTable);

        public async Task RemoveAsync(string id) =>
            await _tableCollection.DeleteOneAsync(x => x.Id == id);
    }
}
