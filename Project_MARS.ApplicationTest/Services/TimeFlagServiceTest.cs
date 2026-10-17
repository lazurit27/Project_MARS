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
    public class TimeFlagServiceTests
    {
        private readonly Mock<ITimeFlagRepository> _timeFlagRepoMock;
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<IRepairBoxRepository> _repairBoxRepoMock;
        private readonly Mock<ILogger<TimeFlagService>> _loggerMock;
        private readonly TimeFlagService _service;

        public TimeFlagServiceTests()
        {
            _timeFlagRepoMock = new Mock<ITimeFlagRepository>();
            _reservationRepoMock = new Mock<IReservationRepository>();
            _repairBoxRepoMock = new Mock<IRepairBoxRepository>();
            _loggerMock = new Mock<ILogger<TimeFlagService>>();

            _service = new TimeFlagService(
                _timeFlagRepoMock.Object,
                _reservationRepoMock.Object,
                _repairBoxRepoMock.Object,
                _loggerMock.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenReservationNotFound()
        {
            // Arrange
            int reserveId = 1;
            _reservationRepoMock.Setup(r => r.GetAsync(reserveId)).ReturnsAsync((Reservation?)null);

            // Act
            var result = await _service.CreateAsync("Delay", TimeFlagType.Resumed, null, ReservationType.inProgress, false, reserveId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Reservation not found");
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenNewEndTimeConflicts()
        {
            // Arrange
            int reserveId = 1;
            int boxId = 5;
            var newEnd = DateTime.UtcNow.AddHours(2);

            var reservation = new Reservation
            {
                Id = reserveId,
                RepairBoxId = boxId,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };

            _reservationRepoMock.Setup(r => r.GetAsync(reserveId)).ReturnsAsync(reservation);
            _reservationRepoMock.Setup(r => r.HasConflictingReservationAsync(boxId, reservation.StartTime, newEnd)).ReturnsAsync(true);

            // Act
            var result = await _service.CreateAsync("Delay", TimeFlagType.Resumed, newEnd, ReservationType.inProgress, false, reserveId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Contain("New time conflicts with an existing reservation");
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_AndDeactivateBox_WhenNoTimeIsTrue()
        {
            // Arrange
            int reserveId = 1;
            int boxId = 5;
            var reservation = new Reservation
            {
                Id = reserveId,
                RepairBoxId = boxId,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(1)
            };
            var box = new RepairBox { Id = boxId, IsActive = true };

            _reservationRepoMock.Setup(r => r.GetAsync(reserveId)).ReturnsAsync(reservation);
            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(box);

            // Act
            var result = await _service.CreateAsync("Maintenance issue", TimeFlagType.Postponed, null, ReservationType.inProgress, true, reserveId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            box.IsActive.Should().BeFalse();
            _repairBoxRepoMock.Verify(r => r.UpdateAsync(box), Times.Once());
            _timeFlagRepoMock.Verify(r => r.AddAsync(It.IsAny<TimeFlag>()), Times.Once());
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenTimeFlagNotFound()
        {
            // Arrange
            int flagId = 1;
            _timeFlagRepoMock.Setup(r => r.GetAsync(flagId)).ReturnsAsync((TimeFlag?)null);

            // Act
            var result = await _service.DeleteAsync(flagId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("TimeFlag not found");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRestoreBoxAndReservation_WhenFlagDeleted()
        {
            // Arrange
            int flagId = 1;
            int reserveId = 10;
            int boxId = 2;

            var timeFlag = new TimeFlag { Id = flagId, ReservationId = reserveId, PostTime = DateTime.UtcNow };
            var reservation = new Reservation { Id = reserveId, RepairBoxId = boxId, EndTime = DateTime.UtcNow.AddHours(2) };
            var box = new RepairBox { Id = boxId, IsActive = false };

            _timeFlagRepoMock.Setup(r => r.GetAsync(flagId)).ReturnsAsync(timeFlag);
            _reservationRepoMock.Setup(r => r.GetAsync(reserveId)).ReturnsAsync(reservation);
            _timeFlagRepoMock.Setup(r => r.GetByReservAsync(reserveId)).ReturnsAsync(new List<TimeFlag>()); // Остатних флагов нет
            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(box);

            // Act
            var result = await _service.DeleteAsync(flagId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            box.IsActive.Should().BeTrue();
            _repairBoxRepoMock.Verify(r => r.UpdateAsync(box), Times.Once());
            _timeFlagRepoMock.Verify(r => r.RemoveAsync(timeFlag), Times.Once());
        }

        #endregion
    }
}