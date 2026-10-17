using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class ManageBossDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = string.Empty;
    }
}
