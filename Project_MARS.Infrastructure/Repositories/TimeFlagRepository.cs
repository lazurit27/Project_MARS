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
    public class TimeFlagRepository(AppDbContext _context) : ITimeFlagRepository
    {
        public async Task<List<TimeFlag>> GetAsync()
        {
            return await _context.TimeFlags.AsNoTracking().ToListAsync();
        }

        public async Task<TimeFlag?> GetAsync(int id)
        {
            return await _context.TimeFlags.FindAsync(id);
        }

        public async Task AddAsync(TimeFlag timeFlag)
        {
            await _context.TimeFlags.AddAsync(timeFlag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TimeFlag timeFlag)
        {
            _context.TimeFlags.Update(timeFlag);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TimeFlag timeFlag)
        {
            _context.TimeFlags.Remove(timeFlag);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TimeFlag>> GetByReservAsync(int reservId)
        {
            return await _context.TimeFlags.AsNoTracking()
                .Where(tf => tf.ReservationId == reservId)
                .ToListAsync();
        }
    }
}
