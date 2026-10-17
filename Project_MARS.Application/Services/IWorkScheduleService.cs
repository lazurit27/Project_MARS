using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IWorkScheduleService
    {
        Task<Result<List<WorkSchedule>>> GetByServiceAsync(int Service);

        Task<Result> UpdateAsync(List<WorkSchedule> schedules);
    }
}
