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
    public class ReservationService(
        IReservationRepository reservationRepository,
        IRepairBoxRepository repairBoxRepository,
        ICarRepository carRepository,
        ILogger<ReservationService> logger) : IReservationService
    {
        public async Task<Result<PagedResult<Reservation>>> GetPagedAsync(int page, int pageSize, int boxId, ReservationType type, ReservationSortColumn sort, bool ascending)
        {
            int skip = (page - 1) * pageSize;
            var items = await reservationRepository.GetPagedAsync(boxId, skip, pageSize, type, sort, ascending);
            int totalCount = await reservationRepository.CountAsync(); 

            var pagedResult = new PagedResult<Reservation>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Result<PagedResult<Reservation>>.Success(pagedResult);
        }

        public async Task<Result<Reservation>> AddAsync(DateTime start, DateTime end, string description, int carId, int boxId)
        {
           
            var box = await repairBoxRepository.GetAsync(boxId);
            if (box == null)
            {
                return Result<Reservation>.Failure("Repair box not found.", ErrorType.NotFound);
            }

            
            if (!box.IsActive)
            {
                return Result<Reservation>.Failure("This repair box is currently inactive.", ErrorType.BadRequest);
            }

          
            var car = await carRepository.GetAsync(carId);
            if (car == null)
            {
                return Result<Reservation>.Failure("Car not found.", ErrorType.NotFound);
            }

           
            if (end <= start)
            {
                int durationMinutes = box.ServiceCategory?.EstimatedDurationMinutes ?? 60; // дефолт 60 минут
                end = start.AddMinutes(durationMinutes);
            }

            bool hasConflict = await reservationRepository.HasConflictingReservationAsync(boxId, start, end);
            if (hasConflict)
            {
                return Result<Reservation>.Failure("The selected time slot is already booked for this repair box.", ErrorType.Conflict);
            }

            var reservation = new Reservation
            {
                StartTime = start,
                EndTime = end,
                Description = description,
                CarId = carId,
                RepairBoxId = boxId,
                Status = ReservationType.Pending 
            };

            await reservationRepository.AddAsync(reservation);
            logger.LogInformation("Reservation created successfully for Car ID {CarId} in Box ID {BoxId}", carId, boxId);

            return Result<Reservation>.Success(reservation);
        }


        public async Task<Result<List<Reservation>>> GetByVinAsync(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return Result<List<Reservation>>.Failure("VIN cannot be empty.", ErrorType.BadRequest);
            }

            var reservations = await reservationRepository.GetByVin(vin.Trim().ToUpper());
            return Result<List<Reservation>>.Success(reservations);
        }
    }
}