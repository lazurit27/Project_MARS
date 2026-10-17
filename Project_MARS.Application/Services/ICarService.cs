using Project_MARS.Application.Common;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public interface ICarService
    {
        Task<Result<List<Car>>> GetUserCarsAsync (string id);
        Task<Result<Car>> AddAsync (string vin, string? UserId);
        Task<Result> UpdateAsync(int id, string? UserId);
        Task<Result<Car?>> GetByVin(string vin);
    }
}
