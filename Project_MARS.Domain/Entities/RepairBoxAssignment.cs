using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class RepairBoxAssignment
    {
        public int Id { get; set; }
        public int RepairBoxId { get; set; }
        public RepairBox RepairBox { get; set; } = null!;
        public string WorkerUserId { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UnassignedAt { get; set; } 
        public bool IsActive { get; set; } = true;
    }
}
