using Microsoft.EntityFrameworkCore;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Infrastructure.Repositories
{
    public class ReservationRepository(AppDbContext _context) : IReservationRepository
    {
        public async Task<List<Reservation>> GetAsync()
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Car)
                .Include(r => r.RepairBox)
                .ToListAsync();
        }

        public async Task<Reservation?> GetAsync(int id)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Car)
                .Include(r => r.RepairBox)
                .Include(r => r.TimeFlags)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Reservation>> GetByVin(string vin)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Car)
                .Include(r => r.RepairBox)
                .Include(r => r.TimeFlags)
                .Where(r => r.Car.Vin == vin).ToListAsync();
        }

        public async Task AddAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasConflictingReservationAsync(int boxId, DateTime start, DateTime end)
        {
            return await _context.Reservations
                .AnyAsync(r => r.RepairBoxId == boxId
                            && r.Status == ReservationType.Pending
                            && r.StartTime < end
                            && r.EndTime > start);
        }

        public async Task<List<Reservation>> GetPagedAsync(
            int boxId,
            int skip,
            int take,
            ReservationType type,
            ReservationSortColumn sort,
            bool ascending)
        {
            var query = _context.Reservations
                .AsNoTracking()
                .Where(r => r.RepairBoxId == boxId)
                .Include(r => r.Car)
                .Include(r => r.RepairBox)
                .AsQueryable();

            query = sort switch
            {
                ReservationSortColumn.DateTime => ascending ? query.OrderBy(r => r.StartTime) : query.OrderByDescending(r => r.StartTime),
                _ => ascending ? query.OrderBy(r => r.Id) : query.OrderByDescending(r => r.Id)
            };

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Reservations.CountAsync();
        }
    }
}

