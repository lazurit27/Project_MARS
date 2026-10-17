using Project_MARS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class TimeFlag
    {
        public int Id { get; set; }

        public DateTime FlagTime { get; set; }
        public DateTime PostTime { get; set; }
        public TimeFlagType Type { get; set; }
        public string Reason { get; set; } = string.Empty;

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
    }
}
