using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Common;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;
using System.Security.Claims;


namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    public class RatesController(IRateService rateService) : ApiBaseController
    {
        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<PagedResult<Rate>>> GetByServiceAsync(
            int serviceId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await rateService.GetByServiceAsync(serviceId, page, pageSize);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }
       
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Rate>> AddAsync([FromBody] CreateRateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await rateService.AddAsync(dto.Rating, dto.Description, userId, dto.ServiceId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await rateService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}