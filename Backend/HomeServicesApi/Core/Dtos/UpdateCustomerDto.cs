using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace Core.Dtos
{
    public class UpdateCustomerDto
    {
        public int CustomerId { get; set; }
        public Address? Address { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }

    }
}
