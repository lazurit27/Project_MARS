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
    public class ServiceCategoryRepository(AppDbContext _context) : IServiceCategoryRepository
    {
        public async Task<List<ServiceCategory>> GetAsync()
        {
            return await _context.ServiceCategories.AsNoTracking().ToListAsync();
        }

        public async Task<ServiceCategory?> GetAsync(int id)
        {
            return await _context.ServiceCategories.FindAsync(id);
        }

        public async Task AddAsync(ServiceCategory serviceCategory)
        {
            await _context.ServiceCategories.AddAsync(serviceCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceCategory serviceCategory)
        {
            _context.ServiceCategories.Update(serviceCategory);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(ServiceCategory serviceCategory)
        {
            _context.ServiceCategories.Remove(serviceCategory);
            await _context.SaveChangesAsync();
        }
    }
}
