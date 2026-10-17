using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class RateService(
        IRateRepository rateRepository,
        IAutoServiceRepository autoServiceRepository,
        IReservationRepository reservationRepository,
        ILogger<RateService> logger) : IRateService
    {
        public async Task<Result<PagedResult<Rate>>> GetByServiceAsync(int serviceId, int page, int pageSize)
        {
            int skip = (page - 1) * pageSize;
            var items = await rateRepository.GetByServiceAsync(serviceId, skip, pageSize);
            int totalCount = await rateRepository.CountAsync();

            var pagedResult = new PagedResult<Rate>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Result<PagedResult<Rate>>.Success(pagedResult);
        }

        public async Task<Result<Rate>> AddAsync(double rating, string description, string userId, int serviceId)
        {
            if (rating < 1.0 || rating > 5.0)
            {
                return Result<Rate>.Failure("Rating must be between 1.0 and 5.0.", ErrorType.BadRequest);
            }

            var autoService = await autoServiceRepository.GetAsync(serviceId);
            if (autoService == null)
            {
                return Result<Rate>.Failure("Auto service not found.", ErrorType.NotFound);
            }

            var allReservations = await reservationRepository.GetAsync();
            bool hasCompletedReservation = allReservations.Any(r =>
                r.Status == ReservationType.Completed &&
                r.RepairBox.AutoServiceId == serviceId &&
                r.Car.UserId == userId);

            if (!hasCompletedReservation)
            {
                return Result<Rate>.Failure("You can only review an auto service after a completed reservation.", ErrorType.Unauthorized);
            }

            var rate = new Rate
            {
                Rating = rating,
                Description = description,
                UserId = userId,
                AutoServiceId = serviceId
            };

            await rateRepository.AddAsync(rate);
            await autoServiceRepository.UpdateRateAsync(serviceId, rating, delete: false);

            logger.LogInformation("Rate added successfully for AutoService ID {ServiceId} by User {UserId}", serviceId, userId);
            return Result<Rate>.Success(rate);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var rate = await rateRepository.GetAsync(id); 
            if (rate == null)
            {
                return Result.Failure("Rate not found.", ErrorType.NotFound);
            }

            int serviceId = rate.AutoServiceId;
            double ratingValue = rate.Rating;

            
            await rateRepository.DeleteAsync(rate);

          
            await autoServiceRepository.UpdateRateAsync(serviceId, ratingValue, delete: true);

            logger.LogInformation("Rate ID {Id} deleted and service rating updated.", id);
            return Result.Success();
        }
    }
}