using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Services;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;


namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public class AdminController(IAdminService adminService) : ApiBaseController
    {
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleDto dto)
        {
            var result = await adminService.AssignRoleAsync(dto.UserId, dto.Role);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}