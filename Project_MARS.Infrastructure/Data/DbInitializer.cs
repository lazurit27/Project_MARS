using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_MARS.Infrastructure.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Применяем миграции
            await context.Database.MigrateAsync();

            // 1. Создание ролей на основе Enum (AppRole)
            foreach (var roleName in Enum.GetNames(typeof(RolesType)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Сидирование категорий услуг, если их нет
            if (!await context.ServiceCategories.AnyAsync())
            {
                var categories = new List<ServiceCategory>
                {
                    new ServiceCategory { Name = "Tire Service", EstimatedDurationMinutes = 45 },
                    new ServiceCategory { Name = "Body Shop", EstimatedDurationMinutes = 180 },
                    new ServiceCategory { Name = "Engine and Transmission Center", EstimatedDurationMinutes = 240 },
                    new ServiceCategory { Name = "Auto Electrics and Diagnostics", EstimatedDurationMinutes = 60 },
                    new ServiceCategory { Name = "Express Service", EstimatedDurationMinutes = 30 }
                };

                context.ServiceCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // 3. Создание базовых пользователей (Admin, Boss, User, Worker), если они еще не созданы
            await SeedUserAsync(userManager, context, "admin@mars.com", "AdminPassword123!", RolesType.Admin.ToString());
            await SeedUserAsync(userManager, context, "user@mars.com", "UserPassword123!", RolesType.User.ToString());
            await SeedUserAsync(userManager, context, "worker@mars.com", "WorkerPassword123!", RolesType.Worker.ToString());

            // Создание специального Босса с автосервисом, боксами и расписанием
            await SeedBossAndServiceAsync(userManager, context);
        }

        private static async Task SeedUserAsync(UserManager<IdentityUser> userManager, AppDbContext context, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private static async Task SeedBossAndServiceAsync(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            string bossEmail = "boss@mars.com";
            var bossUser = await userManager.FindByEmailAsync(bossEmail);

            if (bossUser == null)
            {
                bossUser = new IdentityUser
                {
                    UserName = bossEmail,
                    Email = bossEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(bossUser, "BossPassword123!");
                await userManager.AddToRoleAsync(bossUser, RolesType.Boss.ToString());
            }

            // Проверяем, создан ли уже профиль AutoServiceBoss для этого пользователя
            var bossEntity = await context.AutoServiceBosses.FirstOrDefaultAsync(b => b.UserId == bossUser.Id);
            if (bossEntity == null)
            {
                bossEntity = new AutoServiceBoss
                {
                    UserId = bossUser.Id,
                    FirstName = "Mirzali",
                    LastName = "Cabiyev"
                };

                context.AutoServiceBosses.Add(bossEntity);
                await context.SaveChangesAsync();

                var autoService = new AutoService
                {
                    Name = "MARS Auto Service #1",
                    Address = "Baku, st. Nizami 42",
                    AutoServiceBossId = bossEntity.Id
                };

                context.AutoServices.Add(autoService);
                await context.SaveChangesAsync();

                var schedules = new List<WorkSchedule>();
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    schedules.Add(new WorkSchedule
                    {
                        AutoServiceId = autoService.Id,
                        DayOfWeek = day,
                        OpeningTime = new TimeSpan(9, 0, 0),
                        ClosingTime = new TimeSpan(18, 0, 0),
                        IsClosed = (day == DayOfWeek.Sunday)
                    });
                }
                context.WorkSchedules.AddRange(schedules);

                var tireCategory = await context.ServiceCategories.FirstAsync(c => c.Name == "Tire Service");
                var electricCategory = await context.ServiceCategories.FirstAsync(c => c.Name == "Engine and Transmission Center");

                var boxes = new List<RepairBox>
                {
                    new RepairBox { BoxNumber = "Box №1 (Tire Service)", AutoServiceId = autoService.Id, ServiceCategoryId = tireCategory.Id, Price = 400m },
                    new RepairBox { BoxNumber = "Box №2 (Engine and Transmission)", AutoServiceId = autoService.Id, ServiceCategoryId = electricCategory.Id, Price = 600m }
                };

                context.RepairBoxes.AddRange(boxes);
                await context.SaveChangesAsync();
            }
        }
    }
}