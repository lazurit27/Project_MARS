using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IRateService
    {
        Task<Result<PagedResult<Rate>>> GetByServiceAsync(int serviceId, int page, int pageSize);
        Task<Result<Rate>> AddAsync(double raiting, string description, string userId, int ServiceId);
        Task<Result> DeleteAsync(int Id);
    }
}
