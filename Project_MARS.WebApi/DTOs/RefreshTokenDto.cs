using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
