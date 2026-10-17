using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class CreateAutoServiceDto
    {
        [Required(ErrorMessage = "Auto service name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        public IFormFile? LogoFile { get; set; }
    }
}
