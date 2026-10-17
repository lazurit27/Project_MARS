
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_MARS.Application.Interfaces;
using Project_MARS.WebApi.DTOs;
using System.Security.Claims;

namespace Project_MARS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ApiBaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            var result = await authService.RegisterAsync(request.Username, request.Email, request.Password);
            if (!result.IsSuccess)
                return ToErrorResponse(result);

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var result = await authService.LoginAsync(request.Password, request.Email, request.RememberMe);

            if (!result.IsSuccess)
                return ToErrorResponse(result);

            var response = new AuthResponseDto
            {
                AccessToken = result.Value.AccessToken,
                RefreshToken = result.Value.RefreshToken
            };

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto request)
        {
            var result = await authService.RefreshAsync(request.RefreshToken);
            if (!result.IsSuccess)
                return ToErrorResponse(result);

            var response = new AuthResponseDto
            {
                AccessToken = result.Value.AccessToken,
                RefreshToken = result.Value.RefreshToken
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenDto request)
        {
            var result = await authService.LogoutAsync(request.RefreshToken);
            if (!result.IsSuccess)
                return ToErrorResponse(result);

            return Ok();
        }

    }
}