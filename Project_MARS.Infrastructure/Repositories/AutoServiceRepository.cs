using Microsoft.EntityFrameworkCore;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.Infrastructure.Data;

namespace Project_MARS.Infrastructure.Repositories
{
     public  class AutoServiceRepository(AppDbContext _context) : IAutoServiceRepository
    {


        public async Task<List<AutoService>> GetAsync()
        {
            return await _context.AutoServices.AsNoTracking().Include(x => x.AutoServiceBoss).Include(s => s.WorkSchedules).ToListAsync();
        }

        public async Task<AutoService?> GetAsync(int id)
        {
            return await _context.AutoServices
                .AsNoTracking()
                .Include(x => x.AutoServiceBoss)
                .Include(s => s.RepairBoxes)
                .Include(s => s.WorkSchedules)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(AutoService autoService)
        {
            await _context.AutoServices.AddAsync(autoService);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AutoService autoService)
        {
            _context.AutoServices.Update(autoService);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(AutoService autoService)
        {
            _context.AutoServices.Remove(autoService);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasEmployeesAsync(int id)
        {
            
            return await _context.RepairBoxAssignments
                .AnyAsync(a => a.RepairBox.AutoServiceId == id);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _context.AutoServices.AnyAsync(s => s.Name.ToLower() == name.ToLower() && (excludeId == null || s.Id != excludeId));
            
        }

        public async Task<List<AutoService>> GetPagedAsync(
            int skip,
            int take,
            List<int> categoriesIds,
            AutoServiceSortColumn sort,
            bool ascending,
            string? name)
        {
            var query = _context.AutoServices.AsNoTracking().AsQueryable();

            query = sort switch
            {
                AutoServiceSortColumn.Name => ascending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
                AutoServiceSortColumn.Rating => ascending ? query.OrderBy(s => s.Rating) : query.OrderByDescending(s => s.Rating),
                _ => ascending ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id)
            };

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            if (categoriesIds != null && categoriesIds.Count > 0)
            {
                foreach (var categoryId in categoriesIds)
                {
                    query = query.Where(s => s.ServiceCategories.Any(c => c.Id == categoryId));
                }
            }

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync() =>await _context.AutoServices.CountAsync();

        public async Task UpdateRateAsync(int id, double rate, bool delete)
        {
            var service = await _context.AutoServices
                .Include(s => s.Rates)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null) return;

            
            int currentCount = service.Rates.Count;
            int newCount = delete ? currentCount - 1 : currentCount;

            if (newCount <= 0)
            {
                service.Rating = 0.0;
            }
            else
            {
                
                double totalSum = service.Rates.Sum(r => r.Rating);
                if (delete)
                {
                    totalSum -= rate;
                }
                else
                {
                    totalSum += rate;
                }

                service.Rating = totalSum / newCount;
            }

            _context.AutoServices.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task ActiveDisActive(int id)
        {
            var service = await _context.AutoServices.FindAsync(id);

            if (service == null) return;

            service.IsActive = !service.IsActive;
            _context.AutoServices.Update(service);
            await _context.SaveChangesAsync();

        }

        public async Task<List<AutoService>> GetAsyncByOwner(string ownerId)
        {
            return await _context.AutoServices.AsNoTracking().Include(x => x.AutoServiceBoss).Include(x => x.RepairBoxes).ThenInclude(rb => rb.Reservations).Where(x => x.AutoServiceBoss.UserId == ownerId).ToListAsync();
        }
    }
}
