using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class CreateReservationDto
    {
        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Car ID is required.")]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Repair Box ID is required.")]
        public int BoxId { get; set; }
    }
}
