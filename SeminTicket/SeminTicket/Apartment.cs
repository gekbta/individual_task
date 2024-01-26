using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeminTicket
{
    public class Apartment
    {
        public string Address { get; set; }
        public double Area { get; set; }
        public int NumberOfRooms { get; set; }
        public decimal RentPerDay { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
    }
}
