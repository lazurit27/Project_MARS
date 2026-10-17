using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IRateRepository
    {
        Task<List<Rate>> GetAsync();
        Task<Rate?> GetAsync(int id);
        Task<List<Rate>> GetByServiceAsync(int serviceId, int skip, int take);
        Task AddAsync(Rate rate);
        Task DeleteAsync(Rate rate);
        Task<int> CountAsync();
    }
}
