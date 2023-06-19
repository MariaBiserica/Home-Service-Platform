using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Enums;

namespace DataLayer.Entities
{
    public class Booking : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        public DateTime Date { get; set; }
        public BookingStatus Status { get; set; }
    }
}
