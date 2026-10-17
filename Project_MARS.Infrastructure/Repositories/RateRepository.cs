using Microsoft.EntityFrameworkCore;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Infrastructure.Data;

namespace Project_MARS.Infrastructure.Repositories
{
    public class RateRepository(AppDbContext _context) : IRateRepository
    {
        public async Task AddAsync(Rate rate)
        {
            await _context.Rates.AddAsync(rate);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync() => await _context.Rates.CountAsync();

        public async Task DeleteAsync(Rate car)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Rate>> GetAsync()
        {
            return await _context.Rates.AsNoTracking().ToListAsync();
        }

        public async Task<Rate?> GetAsync(int id)
        {
            return await _context.Rates.FindAsync(id);
        }

        public async Task<List<Rate>> GetByServiceAsync(int serviceId, int skip, int take)
        {
            return await _context.Rates.AsNoTracking().Where(c => c.AutoServiceId == serviceId).OrderByDescending(C => C.Id).Skip(skip).Take(take).ToListAsync();
        }
    }
}
