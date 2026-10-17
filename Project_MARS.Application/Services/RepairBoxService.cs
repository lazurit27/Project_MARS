using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class RepairBoxService(
        IRepairBoxRepository repairBoxRepository,
        IAutoServiceRepository autoServiceRepository,
        IServiceCategoryRepository serviceCategoryRepository,
        IReservationRepository reservationRepository,
        ILogger<RepairBoxService> logger) : IRepairBoxService
    {
        public async Task<Result<RepairBox?>> GetAsync(int id)
        {
            var repairBox = await repairBoxRepository.GetAsync(id);
            if (repairBox == null)
            {
                return Result<RepairBox?>.Failure("Repair box not found.", ErrorType.NotFound);
            }
            return Result<RepairBox?>.Success(repairBox);
        }

        public async Task<Result<List<RepairBox>>> GetByServiceAsync(int serviceId)
        {
            var boxes = await repairBoxRepository.GetByServiceAsync(serviceId);
            return Result<List<RepairBox>>.Success(boxes);
        }

        public async Task<Result<RepairBox>> CreateAsync(string boxNumber, decimal price, int serviceId, int categoryId)
        {
            var autoService = await autoServiceRepository.GetAsync(serviceId);
            if (autoService == null)
            {
                return Result<RepairBox>.Failure("Auto service not found.", ErrorType.NotFound);
            }

            var existingBoxes = await repairBoxRepository.GetByServiceAsync(serviceId);
            if (existingBoxes.Any(b => b.BoxNumber.Trim().ToLower() == boxNumber.Trim().ToLower()))
            {
                return Result<RepairBox>.Failure("A repair box with this number already exists in this auto service.", ErrorType.Conflict);
            }

            bool categoryExistsInService = autoService.ServiceCategories.Any(c => c.Id == categoryId);
            if (!categoryExistsInService)
            {
                
                var categoryToAdd = await serviceCategoryRepository.GetAsync(categoryId);
                if (categoryToAdd == null)
                {
                    return Result<RepairBox>.Failure("service category not found.", ErrorType.NotFound);
                }

                autoService.ServiceCategories.Add(categoryToAdd);
                await autoServiceRepository.UpdateAsync(autoService);
            }

            var repairBox = new RepairBox
            {
                BoxNumber = boxNumber.Trim(),
                Price = price,
                AutoServiceId = serviceId,
                ServiceCategoryId = categoryId
            };

            await repairBoxRepository.AddAsync(repairBox);
            logger.LogInformation("Repair box {BoxNumber} created successfully for service ID {ServiceId}", boxNumber, serviceId);

            return Result<RepairBox>.Success(repairBox);
        }

        public async Task<Result> UpdateAsync(int id, string boxNumber, decimal price, int categoryId)
        {
            var repairBox = await repairBoxRepository.GetAsync(id);
            if (repairBox == null)
            {
                return Result.Failure("Repair box not found.", ErrorType.NotFound);
            }

            int serviceId = repairBox.AutoServiceId;
            int oldCategoryId = repairBox.ServiceCategoryId;

            if (!string.IsNullOrWhiteSpace(boxNumber) && boxNumber != repairBox.BoxNumber)
            {
                var existingBoxes = await repairBoxRepository.GetByServiceAsync(serviceId);
                if (existingBoxes.Any(b => b.Id != id && b.BoxNumber.Trim().ToLower() == boxNumber.Trim().ToLower()))
                {
                    return Result.Failure("Another repair box with this number already exists in this auto service.", ErrorType.Conflict);
                }
                repairBox.BoxNumber = boxNumber.Trim();
            }


            if (price >= 0)
            {
                repairBox.Price = price;
            }

            if (categoryId != oldCategoryId)
            {
                var autoService = await autoServiceRepository.GetAsync(serviceId);
                if (autoService != null)
                {
                    if (!autoService.ServiceCategories.Any(c => c.Id == categoryId))
                    {
                        var categoryToAdd = await serviceCategoryRepository.GetAsync(categoryId);
                        if (categoryToAdd == null)
                        {
                            return Result<RepairBox>.Failure("service category not found.", ErrorType.NotFound);
                        }
                    }

                    var serviceBoxes = await repairBoxRepository.GetByServiceAsync(serviceId);
                    bool isOldCategoryStillUsed = serviceBoxes.Any(b => b.Id != id && b.ServiceCategoryId == oldCategoryId);

                    if (!isOldCategoryStillUsed)
                    {
                        var categoryToRemove = autoService.ServiceCategories.FirstOrDefault(c => c.Id == oldCategoryId);
                        if (categoryToRemove != null)
                        {
                            autoService.ServiceCategories.Remove(categoryToRemove);
                        }
                    }

                    await autoServiceRepository.UpdateAsync(autoService);
                }

                repairBox.ServiceCategoryId = categoryId;
            }

            await repairBoxRepository.UpdateAsync(repairBox);
            logger.LogInformation("Repair box ID {Id} updated successfully.", id);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var repairBox = await repairBoxRepository.GetAsync(id);
            if (repairBox == null)
            {
                return Result.Failure("Repair box not found.", ErrorType.NotFound);
            }

            int serviceId = repairBox.AutoServiceId;
            int categoryId = repairBox.ServiceCategoryId;

            bool hasActiveEmployees = await repairBoxRepository.HasActiveEmployeesAsync(id);
            if (hasActiveEmployees)
            {
                return Result.Failure("Cannot delete repair box with active employees assigned to it.", ErrorType.BadRequest);
            }

            await repairBoxRepository.RemoveAsync(repairBox);

            var remainingBoxes = await repairBoxRepository.GetByServiceAsync(serviceId);
            bool isCategoryStillUsed = remainingBoxes.Any(b => b.ServiceCategoryId == categoryId);

            if (!isCategoryStillUsed)
            {
                var autoService = await autoServiceRepository.GetAsync(serviceId);
                if (autoService != null)
                {
                    var categoryToRemove = autoService.ServiceCategories.FirstOrDefault(c => c.Id == categoryId);
                    if (categoryToRemove != null)
                    {
                        autoService.ServiceCategories.Remove(categoryToRemove);
                        await autoServiceRepository.UpdateAsync(autoService);
                    }
                }
            }

            logger.LogInformation("Repair box ID {Id} deleted successfully.", id);
            return Result.Success();
        }

        public async Task<Result> makeActiveDisactive(int boxId)
        {
            var repairBox = await repairBoxRepository.GetAsync(boxId);
            if (repairBox == null)
            {
                return Result.Failure("Repair box not found.", ErrorType.NotFound);
            }
            if (repairBox.IsActive)
            {
                foreach (var reserve in repairBox.Reservations)
                {
                    if (reserve.Status != ReservationType.canceled)
                    {
                        reserve.Status = ReservationType.canceled;
                        await reservationRepository.UpdateAsync(reserve);
                    }
                }
            }
            await repairBoxRepository.makeActiveDisactive(boxId);
            logger.LogInformation("Repair box ID {Id} Active/Disactive successfully.", boxId);
            return Result.Success();
        }
    }
}