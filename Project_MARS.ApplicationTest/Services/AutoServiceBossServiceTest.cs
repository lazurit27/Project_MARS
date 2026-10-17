using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Project_MARS.Application.Common;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using Project_MARS.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Services
{
    public class AutoServiceBossServiceTests
    {
        private readonly Mock<IAutoServiceBossRepository> _bossRepoMock;
        private readonly Mock<IAutoServiceRepository> _serviceRepoMock;
        private readonly Mock<IRepairBoxRepository> _boxRepoMock;
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<ILogger<AutoServiceBossService>> _loggerMock;
        private readonly AutoServiceBossService _service;

        public AutoServiceBossServiceTests()
        {
            _bossRepoMock = new Mock<IAutoServiceBossRepository>();
            _serviceRepoMock = new Mock<IAutoServiceRepository>();
            _boxRepoMock = new Mock<IRepairBoxRepository>();
            _reservationRepoMock = new Mock<IReservationRepository>();
            _loggerMock = new Mock<ILogger<AutoServiceBossService>>();

            // Настройка мока UserManager через IUserStore
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _service = new AutoServiceBossService(
                _bossRepoMock.Object,
                _serviceRepoMock.Object,
                _boxRepoMock.Object,
                _reservationRepoMock.Object,
                _userManagerMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task MakeBoss_ShouldFail_WhenUserNotFound()
        {
            // Arrange
            string userId = "non-existent";
            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync((IdentityUser?)null);

            // Act
            var result = await _service.MakeBoss(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("User not found");
        }

        [Fact]
        public async Task MakeBoss_ShouldFail_WhenUserIsAlreadyBoss()
        {
            // Arrange
            string userId = "user-1";
            var user = new IdentityUser { Id = userId, UserName = "TestUser" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Boss")).ReturnsAsync(true);

            // Act
            var result = await _service.MakeBoss(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.Conflict);
            result.Error.Should().Contain("User is already an active auto service boss");
        }

        [Fact]
        public async Task MakeBoss_ShouldSucceed_AndCreateNewBoss_WhenUserIsValid()
        {
            // Arrange
            string userId = "user-1";
            var user = new IdentityUser { Id = userId, UserName = "TestUser" };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Boss")).ReturnsAsync(false);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, "Boss")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "User")).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);

            _bossRepoMock.Setup(r => r.GetByUserAsync(userId)).ReturnsAsync((AutoServiceBoss?)null);

            // Act
            var result = await _service.MakeBoss(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _bossRepoMock.Verify(r => r.AddAsync(It.IsAny<AutoServiceBoss>()), Times.Once());
        }

        [Fact]
        public async Task UnMakeBoss_ShouldFail_WhenBossProfileNotFound()
        {
            // Arrange
            string userId = "user-1";
            _bossRepoMock.Setup(r => r.GetByUserAsync(userId)).ReturnsAsync((AutoServiceBoss?)null);

            // Act
            var result = await _service.UnMakeBoss(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(ErrorType.NotFound);
            result.Error.Should().Contain("Boss profile not found for this user");
        }

        [Fact]
        public async Task UnMakeBoss_ShouldSucceed_AndDeactivateServices_WhenBossExists()
        {
            // Arrange
            string userId = "user-1";
            var boss = new AutoServiceBoss { Id = 1, UserId = userId };
            var user = new IdentityUser { Id = userId, UserName = "TestUser" };

            var services = new List<AutoService>
            {
                new()
                {
                    Id = 10,
                    IsActive = true,
                    RepairBoxes = new List<RepairBox>
                    {
                        new()
                        {
                            Id = 100,
                            IsActive = true,
                            Reservations = new List<Reservation>
                            {
                                new() { Id = 1000, Status = ReservationType.Pending }
                            }
                        }
                    }
                }
            };

            _bossRepoMock.Setup(r => r.GetByUserAsync(userId)).ReturnsAsync(boss);
            _userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Boss")).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "Boss")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, "User")).ReturnsAsync(IdentityResult.Success);

            _serviceRepoMock.Setup(s => s.GetAsyncByOwner(userId)).ReturnsAsync(services);

            // Act
            var result = await _service.UnMakeBoss(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _boxRepoMock.Verify(b => b.makeActiveDisactive(100), Times.Once());
            _reservationRepoMock.Verify(r => r.UpdateAsync(It.Is<Reservation>(res => res.Status == ReservationType.canceled)), Times.Once());
            _serviceRepoMock.Verify(s => s.ActiveDisActive(10), Times.Once());
        }
    }
}