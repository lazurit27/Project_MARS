using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class WorkScheduleServiceTests
    {
        private readonly Mock<IWorkScheduleRepository> _workScheduleRepoMock;
        private readonly Mock<IAutoServiceRepository> _autoServiceRepoMock;
        private readonly Mock<ILogger<WorkScheduleService>> _loggerMock;
        private readonly WorkScheduleService _service;

        public WorkScheduleServiceTests()
        {
            _workScheduleRepoMock = new Mock<IWorkScheduleRepository>();
            _autoServiceRepoMock = new Mock<IAutoServiceRepository>();
            _loggerMock = new Mock<ILogger<WorkScheduleService>>();

            _service = new WorkScheduleService(
                _workScheduleRepoMock.Object,
                _autoServiceRepoMock.Object,
                _loggerMock.Object
            );
        }

        #region GetByServiceAsync Tests

        [Fact]
        public async Task GetByServiceAsync_ShouldFail_WhenAutoServiceNotFound()
        {
            // Arrange
            int serviceId = 99;
            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync((AutoService?)null);

            // Act
            var result = await _service.GetByServiceAsync(serviceId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Auto service not found");
        }

        [Fact]
        public async Task GetByServiceAsync_ShouldReturnSchedules_WhenServiceExists()
        {
            // Arrange
            int serviceId = 1;
            var autoService = new AutoService { Id = serviceId };
            var schedules = new List<WorkSchedule>
            {
                new() { Id = 1, AutoServiceId = serviceId, DayOfWeek = DayOfWeek.Monday, IsClosed = false }
            };

            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync(autoService);
            _workScheduleRepoMock.Setup(r => r.GetByServiceAsync(serviceId)).ReturnsAsync(schedules);

            // Act
            var result = await _service.GetByServiceAsync(serviceId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value[0].DayOfWeek.Should().Be(DayOfWeek.Monday);
        }

        #endregion

        #region UpdateAsync Tests

        [Theory]
        [InlineData(null)]
        public async Task UpdateAsync_ShouldFail_WhenSchedulesListIsNullOrEmpty(List<WorkSchedule>? emptyList)
        {
            // Act
            var result = await _service.UpdateAsync(emptyList!);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Schedules list cannot be empty");
        }

        [Fact]
        public async Task UpdateAsync_ShouldFail_WhenOpeningTimeIsLaterThanClosingTime()
        {
            // Arrange
            var schedules = new List<WorkSchedule>
            {
                new()
                {
                    Id = 1,
                    IsClosed = false,
                    OpeningTime = new TimeSpan(18, 0, 0),
                    ClosingTime = new TimeSpan(9, 0, 0) // Открытие позже закрытия
                }
            };

            // Act
            var result = await _service.UpdateAsync(schedules);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Opening time must be earlier than closing time");
        }

        [Fact]
        public async Task UpdateAsync_ShouldSucceed_WhenSchedulesAreValid()
        {
            // Arrange
            var schedules = new List<WorkSchedule>
            {
                new()
                {
                    Id = 1,
                    IsClosed = false,
                    OpeningTime = new TimeSpan(9, 0, 0),
                    ClosingTime = new TimeSpan(18, 0, 0)
                }
            };

            // Act
            var result = await _service.UpdateAsync(schedules);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _workScheduleRepoMock.Verify(r => r.UpdateAsync(It.IsAny<WorkSchedule>()), Times.Once());
        }

        #endregion
    }
}