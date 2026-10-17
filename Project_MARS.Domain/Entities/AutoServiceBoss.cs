using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Domain.Entities
{
    public class AutoServiceBoss
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public List<AutoService> AutoServices { get; set; } = new();
    }
}
