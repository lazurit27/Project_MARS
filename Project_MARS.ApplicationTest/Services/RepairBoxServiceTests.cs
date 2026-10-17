using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class RepairBoxServiceTests
    {
        private readonly Mock<IRepairBoxRepository> _repairBoxRepoMock;
        private readonly Mock<IAutoServiceRepository> _autoServiceRepoMock;
        private readonly Mock<IServiceCategoryRepository> _serviceCategoryRepoMock;
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<ILogger<RepairBoxService>> _loggerMock;
        private readonly RepairBoxService _service;

        public RepairBoxServiceTests()
        {
            _repairBoxRepoMock = new Mock<IRepairBoxRepository>();
            _autoServiceRepoMock = new Mock<IAutoServiceRepository>();
            _serviceCategoryRepoMock = new Mock<IServiceCategoryRepository>();
            _reservationRepoMock = new Mock<IReservationRepository>();
            _loggerMock = new Mock<ILogger<RepairBoxService>>();

            _service = new RepairBoxService(
                _repairBoxRepoMock.Object,
                _autoServiceRepoMock.Object,
                _serviceCategoryRepoMock.Object,
                _reservationRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNotFound_WhenBoxDoesNotExist()
        {
            // Arrange
            int boxId = 99;
            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync((RepairBox?)null);

            // Act
            var result = await _service.GetAsync(boxId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Repair box not found");
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenAutoServiceNotFound()
        {
            // Arrange
            int serviceId = 10;
            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync((AutoService?)null);

            // Act
            var result = await _service.CreateAsync("Box 1", 50, serviceId, 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Auto service not found");
        }

        [Fact]
        public async Task CreateAsync_ShouldFail_WhenBoxNumberAlreadyExistsInService()
        {
            // Arrange
            int serviceId = 1;
            string boxNum = "Box A";
            var autoService = new AutoService { Id = serviceId, ServiceCategories = new List<ServiceCategory>() };
            var existingBoxes = new List<RepairBox> { new() { Id = 1, BoxNumber = boxNum, AutoServiceId = serviceId } };

            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync(autoService);
            _repairBoxRepoMock.Setup(r => r.GetByServiceAsync(serviceId)).ReturnsAsync(existingBoxes);

            // Act
            var result = await _service.CreateAsync(boxNum, 50, serviceId, 1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Contain("A repair box with this number already exists");
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenBoxHasActiveEmployees()
        {
            // Arrange
            int boxId = 1;
            var repairBox = new RepairBox { Id = boxId, AutoServiceId = 1, ServiceCategoryId = 1 };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(repairBox);
            _repairBoxRepoMock.Setup(r => r.HasActiveEmployeesAsync(boxId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(boxId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Cannot delete repair box with active employees");
        }

        [Fact]
        public async Task MakeActiveDisactive_ShouldCancelReservations_WhenBoxIsDeactivated()
        {
            // Arrange
            int boxId = 1;
            var reservation = new Reservation { Id = 10, Status = ReservationType.Pending };
            var repairBox = new RepairBox
            {
                Id = boxId,
                IsActive = true,
                Reservations = new List<Reservation> { reservation }
            };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(repairBox);

            // Act
            var result = await _service.makeActiveDisactive(boxId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            reservation.Status.Should().Be(ReservationType.canceled);
            _reservationRepoMock.Verify(r => r.UpdateAsync(reservation), Times.Once());
            _repairBoxRepoMock.Verify(r => r.makeActiveDisactive(boxId), Times.Once());
        }
    }
}