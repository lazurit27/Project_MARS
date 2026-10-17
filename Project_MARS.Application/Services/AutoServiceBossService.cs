using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class AutoServiceBossService(
        IAutoServiceBossRepository bossRepository,
        IAutoServiceRepository serviceRepository,
        IRepairBoxRepository boxRepo,
        IReservationRepository reservationRepository,
        UserManager<IdentityUser> userManager,
        ILogger<AutoServiceBossService> logger) : IAutoServiceBossService
    {
        public async Task<Result<List<AutoServiceBoss>>> GetAllAsync(int serviceId)
        {
            var bosses = await bossRepository.GetAsync();
            return Result<List<AutoServiceBoss>>.Success(bosses);
        }

        public async Task<Result> MakeBoss(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User not found.", ErrorType.NotFound);
            }

           
            if (await userManager.IsInRoleAsync(user, "Boss"))
            {
                return Result.Failure("User is already an active auto service boss.", ErrorType.Conflict);
            }

         
            var roleResult = await userManager.AddToRoleAsync(user, "Boss");
            if (!roleResult.Succeeded)
            {
                return Result.Failure("Failed to assign 'Boss' role to user.", ErrorType.BadRequest);
            }

            if (await userManager.IsInRoleAsync(user, "User"))
            {
                await userManager.RemoveFromRoleAsync(user, "User");
            }

          
            var existingBoss = await bossRepository.GetByUserAsync(userId);

            if (existingBoss != null)
            {
                
                var services = await serviceRepository.GetAsyncByOwner(userId);
                foreach (var service in services)
                {
                    if (!service.IsActive)
                    {
                        await serviceRepository.ActiveDisActive(service.Id);
                    }

                    foreach (var box in service.RepairBoxes)
                    {
                        if (!box.IsActive)
                        {
                            await boxRepo.makeActiveDisactive(box.Id);
                        }
                    }
                }

                logger.LogInformation("Boss profile for User {UserId} reactivated and services restored.", userId);
            }
            else
            {
              
                var newBoss = new AutoServiceBoss
                {
                    UserId = userId,
                    FirstName = user.UserName ?? "Boss",
                    LastName = ""
                };

                await bossRepository.AddAsync(newBoss);
                logger.LogInformation("New Boss profile created for User {UserId}.", userId);
            }

            return Result.Success();
        }

        public async Task<Result> UnMakeBoss(string userId)
        {
            var boss = await bossRepository.GetByUserAsync(userId);
            if (boss == null)
            {
                return Result.Failure("Boss profile not found for this user.", ErrorType.NotFound);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (await userManager.IsInRoleAsync(user, "Boss"))
                {
                    await userManager.RemoveFromRoleAsync(user, "Boss");
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            var services = await serviceRepository.GetAsyncByOwner(userId);
            foreach (var service in services)
            {
                foreach (var box in service.RepairBoxes)
                {
                    if (box.IsActive)
                    {
                        await boxRepo.makeActiveDisactive(box.Id);
                    }
                    foreach (var reserve in box.Reservations)
                    {
                        if (reserve.Status != ReservationType.canceled)
                        {
                            reserve.Status = ReservationType.canceled;
                            await reservationRepository.UpdateAsync(reserve);
                        }
                    }
                }
                if (service.IsActive)
                {
                    await serviceRepository.ActiveDisActive(service.Id);
                }
            }

            logger.LogInformation("Boss profile for User {UserId} removed, all related services DisActive.", userId);

            return Result.Success();
        }
    }
}