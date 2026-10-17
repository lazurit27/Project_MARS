using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface ITimeFlagRepository
    {
        Task<List<TimeFlag>> GetAsync();
        Task<TimeFlag?> GetAsync(int id);
        Task AddAsync(TimeFlag timeFlag);
        Task UpdateAsync(TimeFlag timeFlag);
        Task RemoveAsync(TimeFlag timeFlag);
        Task<List<TimeFlag>> GetByReservAsync(int reservId);
    }
}
