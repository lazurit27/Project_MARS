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
    public class RepairBoxAssignmentRepository(AppDbContext _context) : IRepairBoxAssignmentRepository
    {

        public async Task<List<RepairBoxAssignment>> GetAsync()
        {
            return await _context.RepairBoxAssignments
                .AsNoTracking()
                .Include(a => a.RepairBox)
                .ToListAsync();
        }

        public async Task<List<RepairBoxAssignment>> GetByServiceAsync(int serviceId)
        {
            return await _context.RepairBoxAssignments
                .AsNoTracking()
                .Include(a => a.RepairBox)
                .Where(a => a.RepairBox.AutoServiceId == serviceId)
                .ToListAsync();
        }

        public async Task<RepairBoxAssignment?> GetAsync(int id)
        {
            return await _context.RepairBoxAssignments
                .AsNoTracking()
                .Include(a => a.RepairBox)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(RepairBoxAssignment repairBoxAssignment)
        {
            await _context.RepairBoxAssignments.AddAsync(repairBoxAssignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RepairBoxAssignment repairBoxAssignment)
        {
            _context.RepairBoxAssignments.Update(repairBoxAssignment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(RepairBoxAssignment repairBoxAssignment)
        {
            _context.RepairBoxAssignments.Remove(repairBoxAssignment);
            await _context.SaveChangesAsync();
        }
    }
}
