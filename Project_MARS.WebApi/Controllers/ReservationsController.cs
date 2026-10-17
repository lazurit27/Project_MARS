using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Common;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.WebApi.Controllers;
using Project_MARS.WebApi.DTOs;

namespace Project_MARS.API.Controllers
{
    public class ReservationsController(IReservationService reservationService) : ApiBaseController
    {
        [Route("api/[controller]")]

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Reservation>> AddAsync([FromBody] CreateReservationDto dto)
        {
            var result = await reservationService.AddAsync(
                dto.StartTime,
                dto.EndTime ?? dto.StartTime,
                dto.Description,
                dto.CarId,
                dto.BoxId
            );

            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

       
        [HttpGet("by-vin/{vin}")]
        [Authorize]
        public async Task<ActionResult<List<Reservation>>> GetByVinAsync(string vin)
        {
            var result = await reservationService.GetByVinAsync(vin);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

        
        [HttpGet("client/box/{boxId}")]
        public async Task<ActionResult<PagedResult<Reservation>>> GetClientReservationsAsync(
            int boxId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await reservationService.GetPagedAsync(
                page,
                pageSize,
                boxId,
                ReservationType.Pending, 
                ReservationSortColumn.DateTime,
                ascending: false
            );

            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }

       
        [HttpGet("management/box/{boxId}")]
        [Authorize(Roles = "Worker")] 
        public async Task<ActionResult<PagedResult<Reservation>>> GetManagedReservationsAsync(
            int boxId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ReservationType type = ReservationType.Pending,
            [FromQuery] ReservationSortColumn sort = ReservationSortColumn.DateTime,
            [FromQuery] bool ascending = true)
        {
            var result = await reservationService.GetPagedAsync(page, pageSize, boxId, type, sort, ascending);
            if (!result.IsSuccess)
            {
                return ToErrorResponse(result);
            }

            return Ok(result.Value);
        }
    }
}