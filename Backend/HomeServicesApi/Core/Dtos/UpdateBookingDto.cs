using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Enums;

namespace Core.Dtos
{
    public class UpdateBookingDto
    {
        public int BookingId { get; set; }
        public BookingStatus Status { get; set; }
    }
}
