using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetAsync();
        Task<Reservation?> GetAsync(int id);
        Task<List<Reservation>> GetByVin(string vin);
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task<bool> HasConflictingReservationAsync(int boxId, DateTime start, DateTime end);

        Task<List<Reservation>> GetPagedAsync(int boxId,int skip, int take, ReservationType type, ReservationSortColumn sort, bool ascending);
        Task<int> CountAsync();
    }
}
