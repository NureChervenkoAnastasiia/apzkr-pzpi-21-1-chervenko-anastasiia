using TastifyAPI.Entities;

namespace TastifyAPI.IServices
{
    public interface IBookingService
    {
        Task<List<Booking>> GetAsync();
        Task<Booking?> GetByIdAsync(string id);
        Task CreateAsync(Booking newBooking);
        Task UpdateAsync(string id, Booking updatedBooking);
        Task DeleteAsync(string id);
        Task<List<Booking>> GetAllBookingsAsync(string guestId);
        Task<Booking> GetByDateAsync(DateTime date);
        Task<List<Booking>> GetSortedByDateAsync(DateTime date);
       
    }
}
