using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class Rate
    {
        public int Id { get; set; }
        public double Rating { get; set; } = 0.0;
        public string Description { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public int AutoServiceId { get; set; } 
        public AutoService AutoService { get; set; } = null!;
    }
}
