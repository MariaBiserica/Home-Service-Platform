using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class CustomersRepository : RepositoryBase<Customer>
    {
        public CustomersRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Customer>> GetByLastNameAsync(string lastName)
        {
            return await DbSet.Where(c => string.Equals(c.LastName, lastName)).ToListAsync();
        }
        public async Task<List<Customer>> GetByFullNameAsync(string firstName, string lastName)
        {
            return await DbSet.Where(c => string.Equals(c.FirstName, firstName)
                                    && string.Equals(c.LastName, lastName)).ToListAsync();
        }
        public async Task<Customer?> GetByEmailAsync(string email)
        {
            //return await Include(c => c.User).Where(c => string.Equals(c.User.Email, email, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
            return await DbSet.Where(c => string.Equals(c.User.Email, email)).FirstOrDefaultAsync();
            //return await Where(c => string.Equals(c.User.Email, email, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
            //return Where(c => c.User.Email == email).FirstOrDefault();
        }
        public async Task<List<Booking>> GetBookingsAsync(int customerId)
        {
            return await DbSet.Where(c => c.Id == customerId).SelectMany(c => c.Bookings).ToListAsync();
        }
        public async Task<List<Booking>> GetBookingsByDateAsync(int customerId, DateTime date)
        {
            return await DbSet.Where(c => c.Id == customerId).SelectMany(c => c.Bookings).Where(b => b.Date == date).ToListAsync();
        }
        public async Task<List<Booking>> GetBookingsByStatusAsync(int customerId, Enums.BookingStatus status)
        {
            return await DbSet.Where(c => c.Id == customerId).SelectMany(c => c.Bookings).Where(b => b.Status == status).ToListAsync();
        }
        public async Task<List<Booking>> GetAllBookingsAsync(int customerId)
        {
            return await DbSet.Where(c => c.Id == customerId).SelectMany(c => c.Bookings).ToListAsync();
        }

        public async void RemoveBookingAsync(int customerId, int bookingId)
        {
            var customer = await GetByIdAsync(customerId);

            var booking = customer.Bookings.FirstOrDefault(b => b.Id == bookingId);

            if (booking != null)
            {
                customer.Bookings.Remove(booking);
            }
            await UpdateAsync(customer);

        }

        
    }
}
