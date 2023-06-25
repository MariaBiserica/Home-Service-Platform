using DataLayer.Entities;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos
{
    public class BookingDisplayDto
    {
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public int ServiceId { get; set; }
        public string ServiceTitle { get; set; }
        public string ProviderEmail { get; set; }
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        public DateTime Date { get; set; }
        public BookingStatus Status { get; set; }
    }
}
