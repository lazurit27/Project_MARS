using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface ITimeFlagService
    {
        Task<Result<List<TimeFlag>>> GetByReserveAsync(int reserveId);
        Task<Result<TimeFlag>> CreateAsync(string reason,TimeFlagType Type, DateTime? end, ReservationType type, bool noTime, int reserveId);

        Task<Result> UpdateAsync(int Id,string reason, TimeFlagType Type, DateTime? end, ReservationType type, bool noTime);
        Task<Result> DeleteAsync(int Id);
    }
}
