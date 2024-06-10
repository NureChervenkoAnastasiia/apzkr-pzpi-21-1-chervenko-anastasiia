using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IStaffService
    {
        Task<List<Staff>> GetAsync();
        Task<Staff> GetByIdAsync(string id);
        Task CreateAsync(Staff staff);
        Task UpdateAsync(string id, Staff updatedStaff);
        Task RemoveAsync(string id);
        Task<bool> AnyAsync(Func<Staff, bool> filter);
        Task<Staff> GetByLoginAsync(string login);
    }
}
