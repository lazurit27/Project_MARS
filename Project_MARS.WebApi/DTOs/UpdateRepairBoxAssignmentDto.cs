using System.ComponentModel.DataAnnotations;

namespace Project_MARS.WebApi.DTOs
{
    public class UpdateRepairBoxAssignmentDto
    {
        [Required(ErrorMessage = "Worker User ID is required.")]
        public string WorkerUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Repair Box ID is required.")]
        public int RepairBoxId { get; set; }
    }
}
