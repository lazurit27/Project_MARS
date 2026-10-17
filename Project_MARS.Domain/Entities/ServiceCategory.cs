using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class ServiceCategory
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int EstimatedDurationMinutes { get; set; }
        public List<AutoService> AutoServices { get; set; } = new();

    }
}
