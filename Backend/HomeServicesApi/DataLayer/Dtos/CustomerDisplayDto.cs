using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos
{
    public class CustomerDisplayDto
    {
        public int UserId { get; set; }
        public UserDisplayDto User { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<BookingDisplayDto>? Bookings { get; set; }
    }
}
