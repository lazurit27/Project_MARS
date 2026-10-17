using Microsoft.EntityFrameworkCore;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Infrastructure.Repositories
{
    public class CarRepository(AppDbContext _context) : ICarRepository
    {
        public async Task<List<Car>> GetAsync()
        {
            return await _context.Cars
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Car?> GetAsync(int id)
        {
            return await _context.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Car>> GetUserCarsAsync(string userId)
        {
            return await _context.Cars
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Car?> GetAsyncByVin(string vin)
        {
            return await _context.Cars
                .AsNoTracking()
                .Include(c => c.Reservations)                 
                .ThenInclude(r => r.TimeFlags)
                .FirstOrDefaultAsync(c => c.Vin == vin);
        }

        public async Task AddAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }
    }
}
