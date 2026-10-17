using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class AutoService
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ServiceLogoUrl { get; set; } = string.Empty;
        public double Rating { get; set; } = 0.0;
        public int TotalReviewsCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public int AutoServiceBossId { get; set; }
        public AutoServiceBoss AutoServiceBoss { get; set; } = null!;

        public List<Rate> Rates { get; set; } = new();
        public List<ServiceCategory> ServiceCategories { get; set; } = new();
        public List<WorkSchedule> WorkSchedules { get; set; } = new();
        public List<RepairBox> RepairBoxes { get; set; } = new();
    }
}
