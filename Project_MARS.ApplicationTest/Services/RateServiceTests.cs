using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Project_MARS.API.Controllers;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Domain.Entities;
using Project_MARS.WebApi.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Project_MARS.UnitTests.Controllers
{
    public class RatesControllerTests
    {
        private readonly Mock<IRateService> _rateServiceMock;
        private readonly RatesController _controller;

        public RatesControllerTests()
        {
            _rateServiceMock = new Mock<IRateService>();
            _controller = new RatesController(_rateServiceMock.Object);
        }

        [Fact]
        public async Task GetByServiceAsync_ShouldReturnOk_WhenServiceSucceeds()
        {
            // Arrange
            int serviceId = 1;
            var pagedResult = new PagedResult<Rate>
            {
                Items = new List<Rate> { new() { Id = 1, Rating = 5, Description = "Great!" } },
                TotalCount = 1,
                Page = 1,
                PageSize = 10
            };

            _rateServiceMock
                .Setup(s => s.GetByServiceAsync(serviceId, 1, 10))
                .ReturnsAsync(Result<PagedResult<Rate>>.Success(pagedResult));

            // Act
            var actionResult = await _controller.GetByServiceAsync(serviceId);

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedData = okResult.Value.Should().BeOfType<PagedResult<Rate>>().Subject;
            returnedData.Items.Should().HaveCount(1);
            returnedData.Items[0].Rating.Should().Be(5);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // Без ClaimsPrincipal
            };

            var dto = new CreateRateDto { ServiceId = 1, Rating = 4, Description = "Good" };

            // Act
            var actionResult = await _controller.AddAsync(dto);

            // Assert
            actionResult.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task AddAsync_ShouldReturnOk_WhenDtoIsValidAndUserAuthorized()
        {
            // Arrange
            string userId = "user-123";
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var dto = new CreateRateDto { ServiceId = 1, Rating = 5, Description = "Excellent service" };
            var createdRate = new Rate { Id = 10, Rating = 5, Description = "Excellent service", UserId = userId, AutoServiceId = 1 };

            _rateServiceMock
                .Setup(s => s.AddAsync(dto.Rating, dto.Description, userId, dto.ServiceId))
                .ReturnsAsync(Result<Rate>.Success(createdRate));

            // Act
            var actionResult = await _controller.AddAsync(dto);

            // Assert
            var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedRate = okResult.Value.Should().BeOfType<Rate>().Subject;
            returnedRate.Rating.Should().Be(5);
            returnedRate.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNoContent_WhenDeletionSucceeds()
        {
            // Arrange
            int rateId = 5;
            _rateServiceMock
                .Setup(s => s.DeleteAsync(rateId))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.DeleteAsync(rateId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}