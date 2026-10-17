using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;


namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    public class TimeFlagsController(ITimeFlagService timeFlagService) : ApiBaseController
    {
        
        [HttpGet("reservation/{reserveId}")]
        public async Task<ActionResult<List<TimeFlag>>> GetByReserveAsync(int reserveId)
        {
            var result = await timeFlagService.GetByReserveAsync(reserveId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        
        [HttpPost]
        [Authorize(Roles = "Worker")]
        public async Task<ActionResult<TimeFlag>> CreateAsync([FromBody] CreateTimeFlagDto dto)
        {
            var result = await timeFlagService.CreateAsync(
                dto.Reason,
                dto.Type,
                dto.End,
                dto.ReservationType,
                dto.NoTime,
                dto.ReserveId
            );

            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTimeFlagDto dto)
        {
            var result = await timeFlagService.UpdateAsync(
                id,
                dto.Reason,
                dto.Type,
                dto.End,
                dto.ReservationType,
                dto.NoTime
            );

            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await timeFlagService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}