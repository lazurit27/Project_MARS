using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;

namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    public class RepairBoxesController(IRepairBoxService repairBoxService) : ApiBaseController
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<RepairBox>> GetByIdAsync(int id)
        {
            var result = await repairBoxService.GetAsync(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<List<RepairBox>>> GetByServiceAsync(int serviceId)
        {
            var result = await repairBoxService.GetByServiceAsync(serviceId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Boss")]
        public async Task<ActionResult<RepairBox>> CreateAsync([FromBody] CreateRepairBoxDto dto)
        {
            var result = await repairBoxService.CreateAsync(dto.BoxNumber, dto.Price, dto.ServiceId, dto.CategoryId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Boss")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateRepairBoxDto dto)
        {
            var result = await repairBoxService.UpdateAsync(id, dto.BoxNumber, dto.Price, dto.CategoryId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Boss")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await repairBoxService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }

        [HttpPatch("{id}/toggle-activity")]
        [Authorize(Roles = "Boss")]
        public async Task<IActionResult> ToggleActivityAsync(int id)
        {
            var result = await repairBoxService.makeActiveDisactive(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}