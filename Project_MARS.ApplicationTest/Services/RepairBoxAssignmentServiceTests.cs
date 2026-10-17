using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class RepairBoxAssignmentServiceTests
    {
        private readonly Mock<IRepairBoxAssignmentRepository> _assignmentRepoMock;
        private readonly Mock<IRepairBoxRepository> _repairBoxRepoMock;
        private readonly Mock<IAutoServiceRepository> _autoServiceRepoMock;
        private readonly Mock<ILogger<RepairBoxAssignmentService>> _loggerMock;
        private readonly RepairBoxAssignmentService _service;

        public RepairBoxAssignmentServiceTests()
        {
            _assignmentRepoMock = new Mock<IRepairBoxAssignmentRepository>();
            _repairBoxRepoMock = new Mock<IRepairBoxRepository>();
            _autoServiceRepoMock = new Mock<IAutoServiceRepository>();
            _loggerMock = new Mock<ILogger<RepairBoxAssignmentService>>();

            _service = new RepairBoxAssignmentService(
                _assignmentRepoMock.Object,
                _repairBoxRepoMock.Object,
                _autoServiceRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task UpdateAsync_ShouldFail_WhenWorkerIdIsNullOrEmpty(string invalidWorkerId)
        {
            // Arrange
            int boxId = 1;
            string bossId = "boss-1";

            // Act
            var result = await _service.UpdateAsync(invalidWorkerId, boxId, bossId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Worker User ID cannot be empty");
        }

        [Theory]
        [InlineData(99)]
        [InlineData(404)]
        public async Task UpdateAsync_ShouldFail_WhenBoxNotFound(int boxId)
        {
            // Arrange
            string workerId = "worker-1";
            string bossId = "boss-1";

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync((RepairBox?)null);

            // Act
            var result = await _service.UpdateAsync(workerId, boxId, bossId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Target repair box not found");
        }

        [Theory]
        [InlineData("hacker-boss", "real-boss")]
        [InlineData("another-boss", "owner-id")]
        public async Task UpdateAsync_ShouldFail_WhenBossDoesNotOwnService(string requestBossId, string actualOwnerId)
        {
            // Arrange
            string workerId = "worker-1";
            int boxId = 1;
            int serviceId = 10;

            var repairBox = new RepairBox { Id = boxId, AutoServiceId = serviceId };
            var autoService = new AutoService
            {
                Id = serviceId,
                AutoServiceBoss = new AutoServiceBoss { UserId = actualOwnerId }
            };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(repairBox);
            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync(autoService);

            // Act
            var result = await _service.UpdateAsync(workerId, boxId, requestBossId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Unauthorized);
            result.Error.Should().Contain("You can only assign workers to boxes within your own auto service");
        }

        [Fact]
        public async Task UpdateAsync_ShouldSucceed_WhenValidDataProvided()
        {
            // Arrange
            string workerId = "worker-1";
            int boxId = 1;
            int serviceId = 10;
            string bossId = "boss-1";

            var repairBox = new RepairBox { Id = boxId, AutoServiceId = serviceId };
            var autoService = new AutoService
            {
                Id = serviceId,
                AutoServiceBoss = new AutoServiceBoss { UserId = bossId }
            };

            _repairBoxRepoMock.Setup(r => r.GetAsync(boxId)).ReturnsAsync(repairBox);
            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync(autoService);
            _assignmentRepoMock.Setup(r => r.GetAsync()).ReturnsAsync(new List<RepairBoxAssignment>());

            // Act
            var result = await _service.UpdateAsync(workerId, boxId, bossId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _assignmentRepoMock.Verify(r => r.AddAsync(It.IsAny<RepairBoxAssignment>()), Times.Once());
        }
    }
}