using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace Core.Dtos
{
    public class BookingDto
    {
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public int? PaymentId { get; set; }


    }
}
