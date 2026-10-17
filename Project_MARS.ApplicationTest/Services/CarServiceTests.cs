using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.DTOs;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class CarServiceTests
    {
        private readonly Mock<ICarRepository> _carRepoMock;
        private readonly Mock<IVinDecoderService> _vinDecoderMock;
        private readonly Mock<ILogger<CarService>> _loggerMock;
        private readonly CarService _service;

        public CarServiceTests()
        {
            _carRepoMock = new Mock<ICarRepository>();
            _vinDecoderMock = new Mock<IVinDecoderService>();
            _loggerMock = new Mock<ILogger<CarService>>();

            _service = new CarService(
                _carRepoMock.Object,
                _vinDecoderMock.Object,
                _loggerMock.Object
            );
        }

        #region AddAsync Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AddAsync_ShouldFail_WhenVinIsEmpty(string? invalidVin)
        {
            // Act
            var result = await _service.AddAsync(invalidVin!, "user-123");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("VIN cannot be empty");
        }

        [Theory]
        [InlineData("SHORTVIN123")]                    
        [InlineData("1HGCR2F83HA000000000")]           
        [InlineData("1HGCR2F83HA00000I")]              
        [InlineData("1HGCR2F83HA00000O")]              
        [InlineData("1HGCR2F83HA00000Q")]             
        public async Task AddAsync_ShouldFail_WhenVinFormatIsInvalid(string invalidVin)
        {
            // Act
            var result = await _service.AddAsync(invalidVin, "user-123");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Invalid VIN format");
        }

        [Fact]
        public async Task AddAsync_ShouldFail_WhenCarAlreadyBelongsToAnotherUser()
        {
            // Arrange
            string validVin = "1HGCR2F83HA123456";
            var existingCar = new Car
            {
                Id = 1,
                Vin = validVin,
                UserId = "owner-999" 
            };

            _carRepoMock.Setup(r => r.GetAsyncByVin(validVin)).ReturnsAsync(existingCar);

            // Act
            var result = await _service.AddAsync(validVin, "user-123");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Contain("This car already belongs to another user");
        }

        [Fact]
        public async Task AddAsync_ShouldClaimCar_WhenCarExistsWithoutOwner()
        {
            // Arrange
            string validVin = "1HGCR2F83HA123456";
            string newUserId = "user-123";
            var existingUnownedCar = new Car
            {
                Id = 1,
                Vin = validVin,
                UserId = string.Empty
            };

            _carRepoMock.Setup(r => r.GetAsyncByVin(validVin)).ReturnsAsync(existingUnownedCar);

            // Act
            var result = await _service.AddAsync(validVin, newUserId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.UserId.Should().Be(newUserId);
            _carRepoMock.Verify(r => r.UpdateAsync(existingUnownedCar), Times.Once());
        }

        [Fact]
        public async Task AddAsync_ShouldFail_WhenVinDecoderReturnsNull()
        {
            // Arrange
            string validVin = "1HGCR2F83HA123456";

            _carRepoMock.Setup(r => r.GetAsyncByVin(validVin)).ReturnsAsync((Car?)null);
            _vinDecoderMock.Setup(v => v.DecodeVinAsync(validVin)).ReturnsAsync((VehicleVinDetailsDto?)null);

            // Act
            var result = await _service.AddAsync(validVin, "user-123");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Failed to decode VIN");
        }

        [Fact]
        public async Task AddAsync_ShouldCreateNewCar_WhenVinIsValidAndDecoded()
        {
            // Arrange
            string validVin = "1HGCR2F83HA123456";
            string userId = "user-123";
            var decodedData = new VehicleVinDetailsDto
            {
                Make = "Honda",
                Model = "Civic",
                Year = 2008
            };

            _carRepoMock.Setup(r => r.GetAsyncByVin(validVin)).ReturnsAsync((Car?)null);
            _vinDecoderMock.Setup(v => v.DecodeVinAsync(validVin)).ReturnsAsync(decodedData);

            // Act
            var result = await _service.AddAsync(validVin, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Brand.Should().Be("Honda");
            result.Value.Model.Should().Be("Civic");
            result.Value.Year.Should().Be(2008);
            result.Value.UserId.Should().Be(userId);

            _carRepoMock.Verify(r => r.AddAsync(It.IsAny<Car>()), Times.Once());
        }

        #endregion

        #region GetByVin Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetByVin_ShouldFail_WhenVinIsEmpty(string? invalidVin)
        {
            // Act
            var result = await _service.GetByVin(invalidVin!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("VIN is required");
        }

        [Fact]
        public async Task GetByVin_ShouldFail_WhenCarNotFound()
        {
            // Arrange
            string vin = "1HGCR2F83HA123456";
            _carRepoMock.Setup(r => r.GetAsyncByVin(vin)).ReturnsAsync((Car?)null);

            // Act
            var result = await _service.GetByVin(vin);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Car not found");
        }

        #endregion
    }
}