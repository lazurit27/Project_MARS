using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IAutoServiceBossService
    {
        Task<Result<List<AutoServiceBoss>>> GetAllAsync(int serviceId);
        Task<Result> MakeBoss(string userId);
        Task<Result> UnMakeBoss(string userId);
    }
}
