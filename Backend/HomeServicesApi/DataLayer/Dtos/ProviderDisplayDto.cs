using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos
{
    public class ProviderDisplayDto
    {
        public int UserId { get; set; }
        public UserDisplayDto User { get; set; }
        public string Bio { get; set; }
        public byte Rating { get; set; }
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public List<ServiceDisplayDto>? Services { get; set; }
    }
}
