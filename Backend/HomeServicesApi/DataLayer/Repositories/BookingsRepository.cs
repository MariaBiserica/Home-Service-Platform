using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class BookingsRepository : RepositoryBase<Booking>
    {
        public BookingsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Booking>> GetBookingsByService(int serviceId)
        {
            return await DbSet.Where(b => b.ServiceId == serviceId).ToListAsync();
        }
        public async Task<List<Booking>> GetBookingsByCustomer(int customerId)
        {
            return await DbSet.Where(b => b.CustomerId == customerId).ToListAsync();
        }
    }
}
