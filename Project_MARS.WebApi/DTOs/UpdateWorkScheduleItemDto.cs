using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class UpdateWorkScheduleItemDto
    {
        [Required(ErrorMessage = "Schedule ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Day of week is required.")]
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan OpeningTime { get; set; }

        public TimeSpan ClosingTime { get; set; }

        public bool IsClosed { get; set; }
    }
}
