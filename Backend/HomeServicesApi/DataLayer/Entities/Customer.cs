using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Customer : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<Booking>? Bookings { get; set; }
    }
}
