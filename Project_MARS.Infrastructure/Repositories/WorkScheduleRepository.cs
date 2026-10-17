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
    public class WorkScheduleRepository(AppDbContext _context) : IWorkScheduleRepository
    {
        public async Task<List<WorkSchedule>> GetAsync()
        {
            return await _context.WorkSchedules.AsNoTracking().ToListAsync();
        }

        public async Task<WorkSchedule?> GetAsync(int id)
        {
            return await _context.WorkSchedules.FindAsync(id);
        }

        public async Task<List<WorkSchedule>> GetByServiceAsync(int serviceId)
        {
            return await _context.WorkSchedules
                .AsNoTracking()
                .Where(ws => ws.AutoServiceId == serviceId)
                .ToListAsync();
        }

        public async Task AddAsync(WorkSchedule workSchedule)
        {
            await _context.WorkSchedules.AddAsync(workSchedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkSchedule workSchedule)
        {
            _context.WorkSchedules.Update(workSchedule);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(WorkSchedule workSchedule)
        {
            _context.WorkSchedules.Remove(workSchedule);
            await _context.SaveChangesAsync();
        }
    }
}
