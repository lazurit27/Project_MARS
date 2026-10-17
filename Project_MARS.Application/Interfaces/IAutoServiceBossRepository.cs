using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IAutoServiceBossRepository
    {
        Task<List<AutoServiceBoss>> GetAsync();
        Task<AutoServiceBoss?> GetAsync(int id);
        public Task<AutoServiceBoss?> GetByUserAsync(string userId);
        Task AddAsync(AutoServiceBoss serviceCategory);
        Task UpdateAsync(AutoServiceBoss serviceCategory);
        Task RemoveAsync(AutoServiceBoss serviceCategory);
    }
}
