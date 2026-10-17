using Project_MARS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class CreateTimeFlagDto
    {
        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(300, ErrorMessage = "Reason cannot exceed 300 characters.")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "TimeFlag type is required.")]
        public TimeFlagType Type { get; set; }

        public DateTime? End { get; set; }

        [Required(ErrorMessage = "Reservation status type is required.")]
        public ReservationType ReservationType { get; set; }

        public bool NoTime { get; set; }

        [Required(ErrorMessage = "Reservation ID is required.")]
        public int ReserveId { get; set; }
    }
}
