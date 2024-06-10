using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IScheduleService
    {
        Task<List<Schedule>> GetAsync();
        Task<Schedule?> GetByIdAsync(string id);
        Task CreateAsync(Schedule newSchedule);
        Task UpdateAsync(string id, Schedule updatedSchedule);
        Task RemoveAsync(string id);
        Task<Schedule?> GetByStaffAsync(string id);

    }
}
