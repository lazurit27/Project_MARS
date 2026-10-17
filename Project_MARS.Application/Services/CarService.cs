using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project_MARS.Application.Services
{
    public class CarService(
        ICarRepository carRepository,
        IVinDecoderService vinDecoderService,
        ILogger<CarService> logger) : ICarService
    {
        
        private static readonly Regex VinRegex = new(@"^[A-HJ-NPR-Z0-9]{17}$", RegexOptions.IgnoreCase);

        public async Task<Result<List<Car>>> GetUserCarsAsync(string userId) 
        {
           
            var cars = await carRepository.GetUserCarsAsync(userId);
            return Result<List<Car>>.Success(cars);
        }

        public async Task<Result<Car>> AddAsync(string vin, string? userId)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return Result<Car>.Failure("VIN cannot be empty.", ErrorType.BadRequest);
            }

            vin = vin.Trim().ToUpper();

            
            if (!VinRegex.IsMatch(vin))
            {
                return Result<Car>.Failure("Invalid VIN format. Must be 17 alphanumeric characters (excluding I, O, Q).", ErrorType.BadRequest);
            }

         
            var existingCar = await carRepository.GetAsyncByVin(vin);
            if (existingCar != null)
            {
                
                if (!string.IsNullOrEmpty(existingCar.UserId))
                {
                    return Result<Car>.Failure("This car already belongs to another user.", ErrorType.Conflict);
                }

               
                if (!string.IsNullOrEmpty(userId))
                {
                    existingCar.UserId = userId;
                    await carRepository.UpdateAsync(existingCar);
                }

                return Result<Car>.Success(existingCar);
            }

         
            var decodedInfo = await vinDecoderService.DecodeVinAsync(vin);
            if (decodedInfo == null)
            {
                return Result<Car>.Failure("Failed to decode VIN or car not found in external registry.", ErrorType.BadRequest);
            }

           
            var newCar = new Car
            {
                Vin = vin,
                Brand = decodedInfo.Make ?? "Unknown",    
                Model = decodedInfo.Model ?? "Unknown",
                Year = decodedInfo.Year,
                UserId = userId ?? string.Empty
            };

            await carRepository.AddAsync(newCar);
            logger.LogInformation("Car with VIN {Vin} successfully added.", vin);

            return Result<Car>.Success(newCar);
        }

        public async Task<Result> UpdateAsync(int id, string? userId)
        {
            var car = await carRepository.GetAsync(id);
            if (car == null)
            {
                return Result.Failure("Car not found.", ErrorType.NotFound);
            }

            car.UserId = userId ?? string.Empty;

            await carRepository.UpdateAsync(car);
            logger.LogInformation("Car ID {Id} owner updated.", id);

            return Result.Success();
        }

        public async Task<Result<Car?>> GetByVin(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
            {
                return Result<Car?>.Failure("VIN is required.", ErrorType.BadRequest);
            }

            var car = await carRepository.GetAsyncByVin(vin.Trim().ToUpper());
            if (car == null)
            {
                return Result<Car?>.Failure("Car not found.", ErrorType.NotFound);
            }

            return Result<Car?>.Success(car);
        }
    }
}