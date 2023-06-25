using DataLayer.Dtos;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Mapping
{
    public static class CustomerMappingExtensions
    {
        public static List<CustomerDisplayDto> ToCustomerDisplayDtos (this List<Customer> customers)
        {
            if (customers == null)
            {
                return null;
            }
            return customers.Select(c => c.ToCustomerDisplayDto()).ToList();
        }

        public static CustomerDisplayDto ToCustomerDisplayDto (this Customer customer)
        {
            if (customer == null)
            {
                return null;
            }

            return new CustomerDisplayDto
            {
                UserId = customer.UserId,
                User = customer.User.ToUserDisplayDto(),
                AddressId = customer.AddressId,
                Address = customer.Address,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Gender = customer.Gender,
                BirthDate = customer.BirthDate,
                Bookings = customer.Bookings.ToBookingDisplayDtos()
            };
        }
    }
}
