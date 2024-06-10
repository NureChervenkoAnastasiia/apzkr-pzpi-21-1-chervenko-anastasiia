using MongoDB.Driver;
using TastifyAPI.Entities;
using TastifyAPI.IServices;

namespace TastifyAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IMongoCollection<Booking> _bookingCollection;

        public BookingService(IMongoDatabase database)
        {
            _bookingCollection = database.GetCollection<Booking>("Bookings");
        }

        public async Task<List<Booking>> GetAsync()
        {
            var bookings = await _bookingCollection.Find(_ => true).ToListAsync();

            foreach (var b in bookings)
            {
                b.BookingDateTime = b.BookingDateTime.ToLocalTime();
            }

            return bookings;
        }

        public async Task<Booking?> GetByIdAsync(string id)
        {
            var booking = await _bookingCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            booking.BookingDateTime = booking.BookingDateTime.ToLocalTime();

            return booking;
        }

        public async Task CreateAsync(Booking newBooking)
        {
            newBooking.BookingDateTime = newBooking.BookingDateTime.ToUniversalTime();

            await _bookingCollection.InsertOneAsync(newBooking);
        }

        public async Task UpdateAsync(string id, Booking updatedBooking)
        {
            updatedBooking.BookingDateTime = updatedBooking.BookingDateTime.ToUniversalTime();

            await _bookingCollection.ReplaceOneAsync(x => x.Id == id, updatedBooking);
        }

        public async Task<Booking> GetByDateAsync(DateTime date)
        {
            date = date.ToUniversalTime();

            var booking = await _bookingCollection.Find(x => x.BookingDateTime == date).FirstOrDefaultAsync();

            return booking;
        }

        public async Task<List<Booking>> GetSortedByDateAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            var sortDefinition = Builders<Booking>.Sort.Descending(x => x.BookingDateTime);

            return await _bookingCollection.Find(x => x.BookingDateTime >= startOfDay && x.BookingDateTime <= endOfDay)
                .Sort(sortDefinition)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllBookingsAsync(string guestId)
        {
            var bookings = await _bookingCollection.Find(x => x.GuestId == guestId).ToListAsync();
            foreach (var booking in bookings)
            {
                booking.BookingDateTime = booking.BookingDateTime.ToLocalTime();
            }
            return bookings;
        }
        public async Task DeleteAsync(string id) =>
            await _bookingCollection.DeleteOneAsync(x => x.Id == id);        
    }
}
