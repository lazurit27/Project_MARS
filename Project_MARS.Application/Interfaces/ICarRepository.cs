using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAsync();
        Task<Car?> GetAsync(int id);
        Task<List<Car>> GetUserCarsAsync(string userId);
        Task<Car?> GetAsyncByVin(string Vin);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
    }
}
