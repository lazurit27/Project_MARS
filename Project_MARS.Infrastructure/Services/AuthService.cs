using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Project_MARS.Domain.Entities;
using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project_MARS.Application.Services
{
    public class AuthService(
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<AuthService> logger,
    ITokenGenerator tokenGenerator,
     UserManager<IdentityUser> userManager,
     SignInManager<IdentityUser> signInManager) : IAuthService
    {
        public async Task<Result<AuthResult>> LoginAsync(string password,string email, bool rememberMe)
        {

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                logger.LogWarning("Login failed: email {Email} not found", email);
                return Result<AuthResult>.Failure("Invalid username or password.", ErrorType.Unauthorized);
            }

            var isVerified = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!isVerified.Succeeded)
            {
                logger.LogWarning("Login failed: wrong password for user with Id {Id}", user.Id);
                return Result<AuthResult>.Failure("Invalid username or password.", ErrorType.Unauthorized);
            }

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var accessToken = tokenGenerator.GenerateAccessToken(user.Id, email, role);
            var refreshToken = tokenGenerator.GenerateRefreshToken();

            await refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken.Token,
                UserId = user.Id,
                ExpiresAt = refreshToken.ExpiresAt
            });

            logger.LogInformation("User {Id} ({Username}) logged in", user.Id, user.UserName);
            return Result<AuthResult>.Success(new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken.Token });
        }
        

        public async Task<Result> LogoutAsync(string refreshAsync)
        {
            var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(refreshAsync);
            if (storedRefreshToken == null)
            {

                return Result.Success();
            }
            logger.LogWarning("Refresh token was deleted");
            await refreshTokenRepository.RemoveAsync(storedRefreshToken);
            return Result.Success();
        }

        public async Task<Result<AuthResult>> RefreshAsync(string refreshAsync)
        {
            var storedRefreshToken = await refreshTokenRepository.GetByTokenAsync(refreshAsync);
            if (storedRefreshToken == null)
            {
                return Result<AuthResult>.Failure("Invalid Refresh Token.", ErrorType.Unauthorized);
            }

            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                logger.LogWarning("Refresh failed: Token is old");
                await refreshTokenRepository.RemoveAsync(storedRefreshToken);
                return Result<AuthResult>.Failure("old Refresh Token.", ErrorType.Unauthorized);
            }

            var user = await userManager.FindByIdAsync(storedRefreshToken.UserId);

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var newAccesToken = tokenGenerator.GenerateAccessToken(storedRefreshToken.UserId,user.UserName , role);
            var refreshToken = tokenGenerator.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken()
            {
                Token = refreshToken.Token,
                UserId = storedRefreshToken.UserId,
                ExpiresAt = refreshToken.ExpiresAt
            };
            await refreshTokenRepository.AddAsync(newRefreshToken);
            await refreshTokenRepository.RemoveAsync(storedRefreshToken);

            var authResult = new AuthResult() { AccessToken = newAccesToken, RefreshToken = refreshToken.Token };
            return Result<AuthResult>.Success(authResult);
        }

        public async Task<Result> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return Result.Failure("User with this email already exists.", ErrorType.Conflict);
            }

            var user = new IdentityUser
            {
                UserName = username,
                Email = email
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors, ErrorType.BadRequest);
            }

            await userManager.AddToRoleAsync(user, RolesType.User.ToString());

            logger.LogInformation("User {Username} registered successfully with role {Role}", username, RolesType.User.ToString());
            return Result.Success();
        }
    }
}
