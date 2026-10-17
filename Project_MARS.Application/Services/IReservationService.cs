using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface IReservationService
    {
        Task<Result<PagedResult<Reservation>>> GetPagedAsync(int page, int pageSize, int boxId, ReservationType type, ReservationSortColumn sort, bool ascending);
        Task<Result<Reservation>> AddAsync(DateTime start, DateTime end, string Desctiption, int CarId, int BoxId);
        Task<Result<List<Reservation>>> GetByVinAsync(string Vin);
    }
}
