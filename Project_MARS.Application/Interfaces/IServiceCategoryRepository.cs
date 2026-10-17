using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IServiceCategoryRepository
    {
        Task<List<ServiceCategory>> GetAsync();
        Task<ServiceCategory?> GetAsync(int id);
        Task AddAsync(ServiceCategory serviceCategory);
        Task UpdateAsync(ServiceCategory serviceCategory);
        Task RemoveAsync(ServiceCategory serviceCategory);

    }
}
