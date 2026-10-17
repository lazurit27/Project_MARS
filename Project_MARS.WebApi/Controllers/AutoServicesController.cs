using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Common;

using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;

using System.Security.Claims;


namespace Project_MARS.API.Controllers
{
    [Route("api/[controller]")]
    public class AutoServicesController(IAutoServiceService autoServiceService) : ApiBaseController
    {
        [HttpGet]
        public async Task<ActionResult<PagedResult<AutoService>>> GetPagedAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] List<int>? categoryIds = null,
            [FromQuery] AutoServiceSortColumn sort = AutoServiceSortColumn.Name,
            [FromQuery] bool ascending = true,
            [FromQuery] string? name = null)
        {
            var result = await autoServiceService.GetPagedAsync(page, pageSize, categoryIds ?? [], sort, ascending, name);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AutoService>> GetByIdAsync(int id)
        {
            var result = await autoServiceService.GetByIdAasync(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Boss")]
        public async Task<ActionResult<AutoService>> AddAsync([FromForm] CreateAutoServiceDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await autoServiceService.AddAsync(dto.Name, dto.Address, dto.LogoFile, userId);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Boss")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] UpdateAutoServiceDto dto)
        {
            var result = await autoServiceService.UpdateAsync(id, dto.Name, dto.Address, dto.LogoFile);
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
            var result = await autoServiceService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return NoContent();
        }
    }
}