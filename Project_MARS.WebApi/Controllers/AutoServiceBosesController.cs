using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;


namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public class AutoServiceBossesController(IAutoServiceBossService bossService) : ApiBaseController
    {
       
        [HttpGet]
        public async Task<ActionResult<List<AutoServiceBoss>>> GetAllAsync([FromQuery] int serviceId = 0)
        {
            var result = await bossService.GetAllAsync(serviceId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

       
        [HttpPost("make-boss")]
        public async Task<IActionResult> MakeBossAsync([FromBody] ManageBossDto dto)
        {
            var result = await bossService.MakeBoss(dto.UserId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }

     
        [HttpPost("unmake-boss")]
        public async Task<IActionResult> UnMakeBossAsync([FromBody] ManageBossDto dto)
        {
            var result = await bossService.UnMakeBoss(dto.UserId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}