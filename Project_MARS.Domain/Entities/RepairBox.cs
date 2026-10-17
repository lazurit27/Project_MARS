using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class RepairBox
    {
        public int Id { get; set; }
        public string BoxNumber { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
        public bool IsActive { get; set; } = true;


        public int AutoServiceId { get; set; }
        public AutoService AutoService { get; set; } = null!;
        public int ServiceCategoryId { get; set; }
        public ServiceCategory ServiceCategory { get; set; } = null!;
        public List<Reservation> Reservations { get; set; } = new();
        public List<RepairBoxAssignment> RepairBoxAssignments { get; set; } = new();
    }
}
