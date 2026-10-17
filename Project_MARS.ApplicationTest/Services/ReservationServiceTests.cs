using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<IRepairBoxRepository> _repairBoxRepoMock;
        private readonly Mock<ICarRepository> _carRepoMock;
        private readonly Mock<ILogger<ReservationService>> _loggerMock;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _reservationRepoMock = new Mock<IReservationRepository>();
            _repairBoxRepoMock = new Mock<IRepairBoxRepository>();
            _carRepoMock = new Mock<ICarRepository>();
            _loggerMock = new Mock<ILogger<ReservationService>>();

            _service = new ReservationService(
                _reservationRepoMock.Object,
                _repairBoxRepoMock.Object,
                _carRepoMock.Object,
                _loggerMock.Object
            );
        }

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldFail_WhenRepairBoxNotFound()
        {
            // Arrange
            int boxId = 1;
            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync((RepairBox?)null);

            // Act
            var result = await _service.AddAsync(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Test", 1, boxId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Repair box not found");
        }

        [Fact]
        public async Task AddAsync_ShouldFail_WhenRepairBoxIsInactive()
        {
            // Arrange
            int boxId = 1;
            var box = new RepairBox { Id = boxId, IsActive = false };
            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(box);

            // Act
            var result = await _service.AddAsync(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Test", 1, boxId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("This repair box is currently inactive");
        }

        [Fact]
        public async Task AddAsync_ShouldFail_WhenCarNotFound()
        {
            // Arrange
            int boxId = 1;
            int carId = 5;
            var box = new RepairBox { Id = boxId, IsActive = true };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(box);
            _carRepoMock.Setup(c => c.GetAsync(carId)).ReturnsAsync((Car?)null);

            // Act
            var result = await _service.AddAsync(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Test", carId, boxId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Car not found");
        }

        [Fact]
        public async Task AddAsync_ShouldFail_WhenTimeSlotHasConflict()
        {
            // Arrange
            int boxId = 1;
            int carId = 5;
            var start = DateTime.UtcNow;
            var end = start.AddHours(1);

            var box = new RepairBox { Id = boxId, IsActive = true };
            var car = new Car { Id = carId, Vin = "1HGCR2F83HA123456" };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(box);
            _carRepoMock.Setup(c => c.GetAsync(carId)).ReturnsAsync(car);
            _reservationRepoMock.Setup(r => r.HasConflictingReservationAsync(boxId, start, end)).ReturnsAsync(true);

            // Act
            var result = await _service.AddAsync(start, end, "Test", carId, boxId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Contain("The selected time slot is already booked");
        }

        [Fact]
        public async Task AddAsync_ShouldSucceed_WhenDataIsValidAndNoConflicts()
        {
            // Arrange
            int boxId = 1;
            int carId = 5;
            var start = DateTime.UtcNow;
            var end = start.AddHours(1);

            var box = new RepairBox { Id = boxId, IsActive = true };
            var car = new Car { Id = carId, Vin = "1HGCR2F83HA123456" };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(box);
            _carRepoMock.Setup(c => c.GetAsync(carId)).ReturnsAsync(car);
            _reservationRepoMock.Setup(r => r.HasConflictingReservationAsync(boxId, start, end)).ReturnsAsync(false);

            // Act
            var result = await _service.AddAsync(start, end, "Engine check", carId, boxId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.CarId.Should().Be(carId);
            result.Value.RepairBoxId.Should().Be(boxId);
            result.Value.Status.Should().Be(ReservationType.Pending);
            _reservationRepoMock.Verify(r => r.AddAsync(It.IsAny<Reservation>()), Times.Once());
        }

        #endregion

        #region GetByVinAsync Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task GetByVinAsync_ShouldFail_WhenVinIsEmpty(string? invalidVin)
        {
            // Act
            var result = await _service.GetByVinAsync(invalidVin!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("VIN cannot be empty");
        }

        [Fact]
        public async Task GetByVinAsync_ShouldReturnReservations_WhenVinIsValid()
        {
            // Arrange
            string vin = "1HGCR2F83HA123456";
            var expectedReservations = new List<Reservation>
            {
                new() { Id = 1, Description = "Fix brakes" }
            };

            _reservationRepoMock
                .Setup(r => r.GetByVin(vin))
                .ReturnsAsync(expectedReservations);

            // Act
            var result = await _service.GetByVinAsync(vin);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].Description.Should().Contain("Fix brakes");
        }

        #endregion
    }
}