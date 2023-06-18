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
        public User User { get; set; }
        public Service Service { get; set; }
        public Payment Payment { get; set; }
        public DateTime Date { get; set; }
        public BookingStatus Status { get; set; }
    }
}
