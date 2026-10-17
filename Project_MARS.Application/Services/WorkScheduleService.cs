using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class WorkScheduleService(
        IWorkScheduleRepository workScheduleRepository,
        IAutoServiceRepository autoServiceRepository,
        ILogger<WorkScheduleService> logger) : IWorkScheduleService
    {
        public async Task<Result<List<WorkSchedule>>> GetByServiceAsync(int serviceId)
        {
            
            var autoService = await autoServiceRepository.GetAsync(serviceId);
            if (autoService == null)
            {
                return Result<List<WorkSchedule>>.Failure("Auto service not found.", ErrorType.NotFound);
            }

            var schedules = await workScheduleRepository.GetByServiceAsync(serviceId);
            return Result<List<WorkSchedule>>.Success(schedules);
        }

        public async Task<Result> UpdateAsync(List<WorkSchedule> schedules)
        {
            if (schedules == null || schedules.Count == 0)
            {
                return Result.Failure("Schedules list cannot be empty.", ErrorType.BadRequest);
            }

            foreach (var schedule in schedules)
            {
               
                if (!schedule.IsClosed && schedule.OpeningTime >= schedule.ClosingTime)
                {
                    return Result.Failure("Opening time must be earlier than closing time.", ErrorType.BadRequest);
                }

                await workScheduleRepository.UpdateAsync(schedule);
            }

            logger.LogInformation("Work schedules updated successfully.");
            return Result.Success();
        }
    }
}