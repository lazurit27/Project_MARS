using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IAutoServiceRepository
    {
        Task<List<AutoService>> GetAsync();
        Task<List<AutoService>> GetAsyncByOwner(string ownerId);
        Task<AutoService?> GetAsync(int id);
        Task AddAsync(AutoService autoService);
        Task UpdateAsync(AutoService autoService);
        Task RemoveAsync(AutoService autoService);
        Task<bool> HasEmployeesAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task UpdateRateAsync(int id, double rate, bool delete);

        Task ActiveDisActive(int id);

        Task<List<AutoService>> GetPagedAsync(int skip, int take, List<int> categoryIds, AutoServiceSortColumn sort, bool ascending, string? name);
        Task<int> CountAsync();
    }
}
