using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IRepairBoxService
    {
        Task<Result<RepairBox?>> GetAsync(int id);

        Task<Result<List<RepairBox>>> GetByServiceAsync(int serviceId);

        Task<Result<RepairBox>> CreateAsync(string boxNumber, decimal price, int serviceId, int categoryId);

        Task<Result> UpdateAsync(int id, string boxNumber, decimal price, int categoryId);

        Task<Result> makeActiveDisactive(int boxId);

        Task<Result> DeleteAsync(int id);
    }
}
