using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class Car
    {
        public int Id { get; set; }         
        public string Vin { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<Reservation> Reservations { get; set; } = new();
    }
}
