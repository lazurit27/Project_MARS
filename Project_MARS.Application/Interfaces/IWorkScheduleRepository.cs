using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IWorkScheduleRepository
    {
        Task<List<WorkSchedule>> GetAsync();
        Task<WorkSchedule?> GetAsync(int id);
        Task<List<WorkSchedule>> GetByServiceAsync(int serviceId);
        Task AddAsync(WorkSchedule workSchedule);
        Task UpdateAsync(WorkSchedule workSchedule);
        Task RemoveAsync(WorkSchedule workSchedule);
    }
}
