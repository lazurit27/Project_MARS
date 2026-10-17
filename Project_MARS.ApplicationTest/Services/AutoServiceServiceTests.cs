using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class AutoServiceServiceTests
    {
        private readonly Mock<IAutoServiceRepository> _autoServiceRepoMock;
        private readonly Mock<IAutoServiceBossRepository> _bossRepoMock;
        private readonly Mock<IFileManager> _fileManagerMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<ILogger<AutoServiceService>> _loggerMock;
        private readonly AutoServiceService _service;

        public AutoServiceServiceTests()
        {
            _autoServiceRepoMock = new Mock<IAutoServiceRepository>();
            _bossRepoMock = new Mock<IAutoServiceBossRepository>();
            _fileManagerMock = new Mock<IFileManager>();
            _loggerMock = new Mock<ILogger<AutoServiceService>>();

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _service = new AutoServiceService(
                _autoServiceRepoMock.Object,
                _bossRepoMock.Object,
                _fileManagerMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotFound_WhenServiceDoesNotExist()
        {
            // Arrange
            int serviceId = 99;
            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync((AutoService?)null);

            // Act
            var result = await _service.GetByIdAasync(serviceId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Auto service not found");
        }

        [Fact]
        public async Task AddAsync_ShouldFail_WhenUserIsNotBoss()
        {
            // Arrange
            string userId = "user-1";
            var user = new IdentityUser { Id = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _service.AddAsync("Turbo Service", "Baku", null, userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Unauthorized);
            result.Error.Should().Contain("Only users with the 'Boss' role can create auto services");
        }

        [Fact]
        public async Task AddAsync_ShouldSucceed_WhenUserIsBossAndDataIsValid()
        {
            // Arrange
            string userId = "boss-user";
            var user = new IdentityUser { Id = userId, UserName = "BossName" };
            var boss = new AutoServiceBoss { Id = 1, UserId = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Boss" });
            _bossRepoMock.Setup(b => b.GetByUserAsync(userId)).ReturnsAsync(boss);
            _autoServiceRepoMock.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

            // Act
            var result = await _service.AddAsync("Elite Motors", "Baku St. 5", null, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Name.Should().Be("Elite Motors");
            result.Value.WorkSchedules.Should().HaveCount(7); 
            _autoServiceRepoMock.Verify(r => r.AddAsync(It.IsAny<AutoService>()), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ShouldFail_WhenServiceHasActiveEmployees()
        {
            // Arrange
            int serviceId = 1;
            var service = new AutoService { Id = serviceId, Name = "Service 1" };

            _autoServiceRepoMock.Setup(r => r.GetAsync(serviceId)).ReturnsAsync(service);
            _autoServiceRepoMock.Setup(r => r.HasEmployeesAsync(serviceId)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(serviceId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.BadRequest);
            result.Error.Should().Contain("Cannot delete auto service with active employees");
        }
    }
}