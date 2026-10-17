
using Project_MARS.Domain.Enums;

namespace Project_MARS.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationType Status { get; set; }
        public string Description { get; set; } = string.Empty;


        public int CarId { get; set; }
        public Car Car { get; set; } = null!;
        public int RepairBoxId { get; set; }
        public RepairBox RepairBox { get; set; } = null!;
        public List<TimeFlag> TimeFlags { get; set; } = new();

    }
}
