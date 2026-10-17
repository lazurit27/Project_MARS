using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class TimeFlagService(
        ITimeFlagRepository timeFlagRepository,
        IReservationRepository reservationRepository,
        IRepairBoxRepository repairBoxRepository,
        ILogger<TimeFlagService> logger) : ITimeFlagService
    {
        public async Task<Result<List<TimeFlag>>> GetByReserveAsync(int reserveId)
        {
            var flags = await timeFlagRepository.GetByReservAsync(reserveId);
            return Result<List<TimeFlag>>.Success(flags);
        }

        public async Task<Result<TimeFlag>> CreateAsync(string reason, TimeFlagType type, DateTime? end, ReservationType reservationType, bool noTime, int reserveId)
        {
            var reservation = await reservationRepository.GetAsync(reserveId);
            if (reservation == null)
            {
                return Result<TimeFlag>.Failure("Reservation not found.", ErrorType.NotFound);
            }

            var timeFlag = new TimeFlag
            {
                Reason = reason,
                Type = type,
                FlagTime = DateTime.UtcNow,
                PostTime = reservation.EndTime, 
                ReservationId = reserveId
            };

            if (end.HasValue)
            {
                bool hasConflict = await reservationRepository.HasConflictingReservationAsync(reservation.RepairBoxId, reservation.StartTime, end.Value);
                if (hasConflict)
                {
                    return Result<TimeFlag>.Failure("New time conflicts with an existing reservation.", ErrorType.Conflict);
                }
                reservation.EndTime = end.Value;
            }

            reservation.Status = reservationType;
            await reservationRepository.UpdateAsync(reservation);

            if (noTime)
            {
                var box = await repairBoxRepository.GetAsync(reservation.RepairBoxId);
                if (box != null && box.IsActive)
                {
                    box.IsActive = false;
                    await repairBoxRepository.UpdateAsync(box);
                }
            }

            await timeFlagRepository.AddAsync(timeFlag);
            logger.LogInformation("TimeFlag created for Reservation ID {ReserveId}", reserveId);

            return Result<TimeFlag>.Success(timeFlag);
        }

        public async Task<Result> UpdateAsync(int id, string reason, TimeFlagType type, DateTime? end, ReservationType reservationType, bool noTime)
        {
            var flag = await timeFlagRepository.GetAsync(id);
            if (flag == null)
            {
                return Result.Failure("TimeFlag not found.", ErrorType.NotFound);
            }

            var reservation = await reservationRepository.GetAsync(flag.ReservationId);
            if (reservation == null)
            {
                return Result.Failure("Associated reservation not found.", ErrorType.NotFound);
            }
            flag.Reason = reason;
            flag.Type = type;

            if (end.HasValue)
            {
                bool hasConflict = await reservationRepository.HasConflictingReservationAsync(reservation.RepairBoxId, reservation.StartTime, end.Value);
                if (hasConflict)
                {
                    return Result.Failure("Updated time conflicts with an existing reservation.", ErrorType.Conflict);
                }
                reservation.EndTime = end.Value;
            }

            reservation.Status = reservationType;
            await reservationRepository.UpdateAsync(reservation);

            if (noTime)
            {
                var box = await repairBoxRepository.GetAsync(reservation.RepairBoxId);
                if (box != null && box.IsActive)
                {
                    box.IsActive = false;
                    await repairBoxRepository.UpdateAsync(box);
                }
            }

            await timeFlagRepository.UpdateAsync(flag);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var flag = await timeFlagRepository.GetAsync(id);
            if (flag == null)
            {
                return Result.Failure("TimeFlag not found.", ErrorType.NotFound);
            }

            int reserveId = flag.ReservationId;
            var reservation = await reservationRepository.GetAsync(reserveId);
            if (reservation == null)
            {
                return Result.Failure("Associated reservation not found.", ErrorType.NotFound);
            }
            var allFlags = await timeFlagRepository.GetByReservAsync(reserveId);
            var remainingFlags = allFlags.Where(f => f.Id != id).OrderByDescending(f => f.FlagTime).ToList();

            if (remainingFlags.Count > 0)
            {
                var lastFlag = remainingFlags.First();
                reservation.EndTime = lastFlag.PostTime;
            }
            else
            {
                
                reservation.EndTime = flag.PostTime;
                reservation.Status = ReservationType.inProgress;
            }


            var box = await repairBoxRepository.GetAsync(reservation.RepairBoxId);
            if (box != null && !box.IsActive)
            {
                box.IsActive = true;
                await repairBoxRepository.UpdateAsync(box);
            }

            await reservationRepository.UpdateAsync(reservation);
            await timeFlagRepository.RemoveAsync(flag);

            logger.LogInformation("TimeFlag ID {Id} deleted and Reservation ID {ReserveId} restored.", id, reserveId);
            return Result.Success();
        }
    }
}