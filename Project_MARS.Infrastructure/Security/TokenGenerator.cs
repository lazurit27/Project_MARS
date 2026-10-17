using Project_MARS.Application.Common;
using Project_MARS.Application.Interfaces;
using Microsoft.Extensions.Options;

using Microsoft.IdentityModel.Tokens;


using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Project_MARS.Infrastructure.Security
{
    public class TokenGenerator(IOptions<JwtOptions> options) : ITokenGenerator
    {
        private readonly JwtOptions jwtOptions = options.Value;
        public string GenerateAccessToken(string UserId, string UserMail, string userRole)
        {
            var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier,UserId),
            new (ClaimTypes.Email ,UserMail ),
            new (ClaimTypes.Role,userRole),
        };

            var configKey = jwtOptions.Key;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = jwtOptions.ExpiryInMinutes;

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshTokenResult GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(bytes);
            return new RefreshTokenResult
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(jwtOptions.RefreshTokenExpiryDays)
            };
        }
    }

}
