
using Microsoft.AspNetCore.Identity;
using Project_MARS.Application.Common;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class AdminService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : IAdminService
    {
        public async Task<Result> AssignRoleAsync(string userId, RolesType roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return Result.Failure("User not found", ErrorType.NotFound);

            if (!await roleManager.RoleExistsAsync(roleName.ToString()))
                return Result.Failure("Role does not exist", ErrorType.BadRequest);

            var result = await userManager.AddToRoleAsync(user, roleName.ToString());
            return result.Succeeded ? Result.Success() : Result.Failure("Failed to assign role", ErrorType.BadRequest);
        }
    }
}
