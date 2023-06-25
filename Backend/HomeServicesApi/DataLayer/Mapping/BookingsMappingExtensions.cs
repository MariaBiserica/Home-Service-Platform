using DataLayer.Dtos;
using DataLayer.Entities;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mapping
{
    public static class BookingsMappingExtensions
    {
        public static List<BookingDisplayDto> ToBookingDisplayDtos(this List<Booking> bookings)
        {
            if (bookings == null)
            {
                return null;
            }

            var results = bookings.Select(b => b.ToBookingDisplayDto()).ToList();
            return results;
        }

        public static BookingDisplayDto ToBookingDisplayDto(this Booking booking)
        {
            if (booking == null)
            {
                return null;
            }

            return new BookingDisplayDto
            {
                CustomerId = booking.CustomerId,
                CustomerFullName = booking.Customer.FirstName + " " + booking.Customer.LastName,
                CustomerEmail = booking.Customer.User.Email,
                ServiceId = (int)booking.ServiceId,
                ServiceTitle = booking.Service.Title,
                ProviderEmail = booking.Service.Provider.User.Email,
                PaymentId = booking.PaymentId,
                Payment = booking.Payment,
                Date = booking.Date,
                Status = booking.Status,
            };
        }
    }
}
