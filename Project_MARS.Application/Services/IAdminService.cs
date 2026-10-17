using Project_MARS.Application.Common;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IAdminService
    {
        Task<Result> AssignRoleAsync(string userId, RolesType roleName);
    }
}
