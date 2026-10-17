using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IRepairBoxAssignmentRepository
    {
        Task<List<RepairBoxAssignment>> GetAsync();
        Task<List<RepairBoxAssignment>> GetByServiceAsync(int serviceId);
        Task<RepairBoxAssignment?> GetAsync(int id);
        Task AddAsync(RepairBoxAssignment repairBoxAssignment);
        Task UpdateAsync(RepairBoxAssignment repairBoxAssignment);
        Task RemoveAsync(RepairBoxAssignment repairBoxAssignment);
    }
}
