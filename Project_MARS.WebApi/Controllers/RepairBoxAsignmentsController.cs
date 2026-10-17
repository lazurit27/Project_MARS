using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;

using System.Security.Claims;

namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Boss")] 
    public class RepairBoxAssignmentsController(IRepairBoxAssignmentService assignmentService) : ApiBaseController
    {

        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<List<RepairBoxAssignment>>> GetByServiceAsync(int serviceId)
        {
            var result = await assignmentService.GetByServiceAsync(serviceId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpDelete("unassign/{workerUserId}")]
        public async Task<IActionResult> UnassignWorkerAsync(string workerUserId)
        {
            var bossUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(bossUserId))
            {
                return Unauthorized();
            }

            var result = await assignmentService.UnassignWorkerAsync(workerUserId, bossUserId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateRepairBoxAssignmentDto dto)
        {
            var bossUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(bossUserId))
            {
                return Unauthorized();
            }

            var result = await assignmentService.UpdateAsync(dto.WorkerUserId, dto.RepairBoxId, bossUserId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}