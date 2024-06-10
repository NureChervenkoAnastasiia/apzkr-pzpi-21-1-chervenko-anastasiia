using MongoDB.Driver;
using TastifyAPI.Entities;

namespace TastifyAPI.Services
{
    public class ScheduleService
    {
        private readonly IMongoCollection<Schedule> _scheduleCollection;

        public ScheduleService(IMongoDatabase database)
        {
            _scheduleCollection = database.GetCollection<Schedule>("Schedule");
        }

        public async Task<List<Schedule?>> GetAsync()
        {
            var schedules = await _scheduleCollection.Find(_ => true).ToListAsync();

            foreach(var s in schedules)
            {
                s.StartDateTime = s.StartDateTime?.ToLocalTime();
                s.FinishDateTime = s.FinishDateTime?.ToLocalTime();
            }

            return schedules;
        }

        public async Task<Schedule?> GetByIdAsync(string id)
        {
            var schedule = await _scheduleCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            schedule.StartDateTime = schedule.StartDateTime?.ToLocalTime();
            schedule.FinishDateTime = schedule.FinishDateTime?.ToLocalTime();

            return schedule;
        }

        public async Task CreateAsync(Schedule newSchedule)
        {
            newSchedule.StartDateTime = newSchedule.StartDateTime?.ToUniversalTime();
            newSchedule.FinishDateTime = newSchedule.FinishDateTime?.ToUniversalTime();

            await _scheduleCollection.InsertOneAsync(newSchedule);
        }

        public async Task UpdateAsync(string id, Schedule updatedSchedule)
        {
            updatedSchedule.StartDateTime = updatedSchedule.StartDateTime?.ToUniversalTime();
            updatedSchedule.FinishDateTime = updatedSchedule.FinishDateTime?.ToUniversalTime();

            await _scheduleCollection.ReplaceOneAsync(x => x.Id == id, updatedSchedule);
        }

        public async Task RemoveAsync(string id) =>
            await _scheduleCollection.DeleteOneAsync(x => x.Id == id);
        public async Task<Schedule?> GetByStaffAsync(string id) =>
            await _scheduleCollection.Find(x => x.StaffId == id).FirstOrDefaultAsync();

    }
}
