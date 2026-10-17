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
    public class RepairBoxRepository(AppDbContext _context) : IRepairBoxRepository
    {
        public async Task<RepairBox?> GetAsync(int id)
        {
            return await _context.RepairBoxes
                .AsNoTracking()
                .Include(b => b.AutoService)
                .Include(b => b.Reservations)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(RepairBox repairBox)
        {
            await _context.RepairBoxes.AddAsync(repairBox);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RepairBox repairBox)
        {
            _context.RepairBoxes.Update(repairBox);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(RepairBox repairBox)
        {
            _context.RepairBoxes.Remove(repairBox);
            await _context.SaveChangesAsync();
        }

        public async Task makeActiveDisactive(int boxId)
        {
            var box = await _context.RepairBoxes.FindAsync(boxId);
            box.IsActive = !box.IsActive;
        }

        public async Task<bool> HasActiveEmployeesAsync(int id)
        {
            return await _context.RepairBoxAssignments.AnyAsync(a => a.RepairBoxId == id && a.IsActive == true);
        }

        public async Task<List<RepairBox>> GetByServiceAsync(int serviceId)
        {
            return await _context.RepairBoxes
                .Where(b => b.AutoServiceId == serviceId)
                .ToListAsync();
        }

    }
}
