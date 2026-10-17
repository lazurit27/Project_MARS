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
    public class CarsController(ICarService carService) : ApiBaseController
    {
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Car>> AddAsync([FromBody] AddCarDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await carService.AddAsync(dto.Vin, userId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<ActionResult<List<Car>>> GetUserCarsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await carService.GetUserCarsAsync(userId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpGet("by-vin/{vin}")]
        public async Task<ActionResult<Car>> GetByVin(string vin)
        {
            var result = await carService.GetByVin(vin);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}/owner")]
        [Authorize]
        public async Task<IActionResult> UpdateOwnerAsync(int id, [FromBody] UpdateCarOwnerDto dto)
        {
            var result = await carService.UpdateAsync(id, dto.UserId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}