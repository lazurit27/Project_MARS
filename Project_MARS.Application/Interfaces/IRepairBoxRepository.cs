using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IRepairBoxRepository
    {
        Task<RepairBox?> GetAsync(int id);
        Task AddAsync(RepairBox repairBox);
        Task UpdateAsync(RepairBox repairBox);
        Task RemoveAsync(RepairBox repairBox);
        Task<bool> HasActiveEmployeesAsync(int id);
        Task makeActiveDisactive(int boxId);
        Task<List<RepairBox>> GetByServiceAsync(int serviceId);
    }
}
