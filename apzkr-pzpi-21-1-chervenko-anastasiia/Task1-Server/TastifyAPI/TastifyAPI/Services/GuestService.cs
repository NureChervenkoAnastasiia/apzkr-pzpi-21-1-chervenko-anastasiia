using MongoDB.Driver;
using System.Linq.Expressions;
using TastifyAPI.Entities;
using TastifyAPI.DTOs;
using TastifyAPI.IServices;
using AutoMapper;

namespace TastifyAPI.Services
{
    public class GuestService : IGuestService
    {
        private readonly IMongoCollection<Guest> _guestCollection;
        private readonly IMapper _mapper;

        public GuestService(IMongoDatabase database, IMapper mapper)
        {
            _guestCollection = database.GetCollection<Guest>("Guests");
            _mapper = mapper;
        }

        public async Task<List<Guest>> GetAsync() =>
            await _guestCollection.Find(_ => true).ToListAsync();

        public async Task<Guest?> GetByIdAsync(string id) =>
            await _guestCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<bool> AnyAsync(Expression<Func<Guest, bool>> filter) =>
            await _guestCollection.Find(filter).AnyAsync();

        public async Task<Guest> GetByLoginAsync(string login) =>
            await _guestCollection.Find(guest => guest.Login == login).FirstOrDefaultAsync();

        public async Task<List<Guest>> GetSortedByBonusAndNameAsync()
        {
            return await _guestCollection.Find(_ => true)
                .SortBy(g => g.Bonus)
                .ThenBy(g => g.Name)
                .ToListAsync();
        }

        public async Task CreateAsync(Guest newGuest)
        {
            var guest = _mapper.Map<Guest>(newGuest);
            await _guestCollection.InsertOneAsync(guest);
        }

        public async Task UpdateAsync(string id, Guest updatedGuest)
        {
            var guest = _mapper.Map<Guest>(updatedGuest);
            guest.Id = id;
            await _guestCollection.ReplaceOneAsync(x => x.Id == id, guest);
        }
        public async Task<(int discount, int remainingBonus)> CalculateCouponAsync(int bonus)
        {
            await Task.CompletedTask;

            decimal bonusCoefficient = 0.7m;
            int discount = 0;

            if (bonus < 100)
            {
                return (discount, bonus);
            }

            if (bonus <= 200)
            {
                bonusCoefficient = 0.5m;
            }
            else if (bonus <= 300)
            {
                bonusCoefficient = 0.6m;
            }

            discount = (int)(bonus * bonusCoefficient);
            int remainingBonus = bonus - discount;

            return (discount, remainingBonus);
        }

        public async Task RemoveAsync(string id) =>
            await _guestCollection.DeleteOneAsync(x => x.Id == id);
    }
}
