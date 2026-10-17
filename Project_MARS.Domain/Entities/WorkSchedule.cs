using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class WorkSchedule
    {
        public int Id { get; set; }

        public int AutoServiceId { get; set; }
        public AutoService AutoService { get; set; } = null!;

        
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan OpeningTime { get; set; } 
        public TimeSpan ClosingTime { get; set; } 

        public bool IsClosed { get; set; }  
    }
}
