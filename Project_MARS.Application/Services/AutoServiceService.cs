using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class AutoServiceService(
        IAutoServiceRepository autoServiceRepository,
        IAutoServiceBossRepository bossRepository,
        IFileManager fileManager,
        UserManager<IdentityUser> userManager,
        ILogger<AutoServiceService> logger) : IAutoServiceService
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;
        public async Task<Result<PagedResult<AutoService>>> GetPagedAsync(int page, int pageSize, List<int> categoryIds, AutoServiceSortColumn sort, bool ascending, string? name)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = DefaultPageSize;

            if (pageSize > MaxPageSize)
                pageSize = MaxPageSize;

            int skip = (page - 1) * pageSize;
            var items = await autoServiceRepository.GetPagedAsync(skip, pageSize, categoryIds, sort, ascending, name);
            var totalCount = await autoServiceRepository.CountAsync();

            var pagedResult = new PagedResult<AutoService>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Result<PagedResult<AutoService>>.Success(pagedResult);
        }

        public async Task<Result<AutoService?>> GetByIdAasync(int id)
        {
            var service = await autoServiceRepository.GetAsync(id);
            if (service == null)
            {
                return Result<AutoService?>.Failure("Auto service not found.", ErrorType.NotFound);
            }
            return Result<AutoService?>.Success(service);
        }

        public async Task<Result<AutoService>> AddAsync(string name, string address, IFormFile? logoFile, string userId)
        {
            
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<AutoService>.Failure("User not found.", ErrorType.Unauthorized);
            }

            var roles = await userManager.GetRolesAsync(user);
            if (!roles.Contains("Boss"))
            {
                return Result<AutoService>.Failure("Only users with the 'Boss' role can create auto services.", ErrorType.Unauthorized);
            }



            var boss = await bossRepository.GetByUserAsync(user.Id);

            if (boss == null)
            {
                
                boss = new AutoServiceBoss
                {
                    UserId = userId,
                    FirstName = user.UserName ?? "Boss",
                    LastName = ""
                };
                await bossRepository.AddAsync(boss);
            }

            var nameExists = await autoServiceRepository.ExistsByNameAsync(name);
            if (nameExists)
            {
                var existingServices = await autoServiceRepository.GetAsync();
                var sameNameService = existingServices.FirstOrDefault(s => s.Name.ToLower() == name.ToLower());

                if (sameNameService != null && sameNameService.AutoServiceBossId != boss.Id)
                {
                    return Result<AutoService>.Failure("An auto service with this name already exists.", ErrorType.Conflict);
                }
            }


            string logoUrl = string.Empty;
            if (logoFile != null)
            {
                logoUrl = await fileManager.SaveFileAsync(logoFile); 
            }


            var defaultWorkSchedules = new List<WorkSchedule>();
            for (int i = 1; i <= 5; i++)
            {
                defaultWorkSchedules.Add(new WorkSchedule
                {
                    DayOfWeek = (DayOfWeek)i,
                    OpeningTime = new TimeSpan(9, 0, 0),
                    ClosingTime = new TimeSpan(18, 0, 0),
                    IsClosed = false
                });
            }

            defaultWorkSchedules.Add(new WorkSchedule { DayOfWeek = DayOfWeek.Saturday, IsClosed = true, OpeningTime = TimeSpan.Zero, ClosingTime = TimeSpan.Zero });
            defaultWorkSchedules.Add(new WorkSchedule { DayOfWeek = DayOfWeek.Sunday, IsClosed = true, OpeningTime = TimeSpan.Zero, ClosingTime = TimeSpan.Zero });

            var autoService = new AutoService
            {
                Name = name,
                Address = address,
                ServiceLogoUrl = logoUrl,
                AutoServiceBossId = boss.Id,
                Rating = 0.0,
                TotalReviewsCount = 0,
                WorkSchedules = defaultWorkSchedules
            };

            await autoServiceRepository.AddAsync(autoService);
            logger.LogInformation("AutoService {Name} created successfully by Boss {BossId}", name, boss.Id);

            return Result<AutoService>.Success(autoService);
        }

        public async Task<Result> UpdateAsync(int id, string name, string address, IFormFile? logoFile)
        {
            var service = await autoServiceRepository.GetAsync(id);
            if (service == null)
            {
                return Result.Failure("Auto service not found.", ErrorType.NotFound);
            }


            if (!string.IsNullOrWhiteSpace(name) && name != service.Name)
            {
                var nameExists = await autoServiceRepository.ExistsByNameAsync(name, id);
                if (nameExists)
                {
                    return Result.Failure("Another auto service with this name already exists.", ErrorType.Conflict);
                }
                service.Name = name;
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                service.Address = address;
            }


            if (logoFile != null)
            {
                service.ServiceLogoUrl = await fileManager.SaveFileAsync(logoFile);
            }

            await autoServiceRepository.UpdateAsync(service);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var service = await autoServiceRepository.GetAsync(id);
            if (service == null)
            {
                return Result.Failure("Auto service not found.", ErrorType.NotFound);
            }


            bool hasEmployees = await autoServiceRepository.HasEmployeesAsync(id);
            if (hasEmployees)
            {
                return Result.Failure("Cannot delete auto service with active employees.", ErrorType.BadRequest);
            }

            if (!string.IsNullOrEmpty(service.ServiceLogoUrl))
            {
                fileManager.DeleteFile(service.ServiceLogoUrl);
            }

            await autoServiceRepository.RemoveAsync(service);
            return Result.Success();
        }
    }
}