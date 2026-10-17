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
    public class AutoServiceBossRepository(AppDbContext _context) : IAutoServiceBossRepository
    {
        public async Task<List<AutoServiceBoss>> GetAsync()
        {
            return await _context.AutoServiceBosses
                .AsNoTracking()
                .Include(b => b.AutoServices)
                .ToListAsync();
        }

        public async Task<AutoServiceBoss?> GetAsync(int id)
        {
            return await _context.AutoServiceBosses
                .AsNoTracking()
                .Include(b => b.AutoServices)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<AutoServiceBoss?> GetByUserAsync(string userId)
        {
            return await _context.AutoServiceBosses
                .AsNoTracking()
                .Include(b => b.AutoServices)
                .FirstOrDefaultAsync(b => b.UserId == userId);
        }

        public async Task AddAsync(AutoServiceBoss autoServiceBoss)
        {
            await _context.AutoServiceBosses.AddAsync(autoServiceBoss);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AutoServiceBoss autoServiceBoss)
        {
            _context.AutoServiceBosses.Update(autoServiceBoss);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(AutoServiceBoss autoServiceBoss)
        {
            _context.AutoServiceBosses.Remove(autoServiceBoss);
            await _context.SaveChangesAsync();
        }
    }
}
