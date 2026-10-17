using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;


namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    public class WorkSchedulesController(IWorkScheduleService workScheduleService) : ApiBaseController
    {
        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<List<WorkSchedule>>> GetByServiceAsync(int serviceId)
        {
            var result = await workScheduleService.GetByServiceAsync(serviceId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

       
        [HttpPut]
        [Authorize(Roles = "Boss")]
        public async Task<IActionResult> UpdateAsync([FromBody] List<UpdateWorkScheduleItemDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest("Schedules list cannot be empty.");
            }

            var schedules = dtos.Select(dto => new WorkSchedule
            {
                Id = dto.Id,
                DayOfWeek = dto.DayOfWeek,
                OpeningTime = dto.OpeningTime,
                ClosingTime = dto.ClosingTime,
                IsClosed = dto.IsClosed
            }).ToList();

            var result = await workScheduleService.UpdateAsync(schedules);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}