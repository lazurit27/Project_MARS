using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IRepairBoxAssignmentService
    {
        Task<Result<List<RepairBoxAssignment>>> GetByServiceAsync(int serviceId);
        Task<Result> UnassignWorkerAsync(string workerUserId, string currentBossUserId);
        Task<Result> UpdateAsync(string workerUserId, int repairBoxId, string currentBossUserId);
    }
}
