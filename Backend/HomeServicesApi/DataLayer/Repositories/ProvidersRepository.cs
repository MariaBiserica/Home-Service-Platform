using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class ProvidersRepository : RepositoryBase<Provider>
    {
        public ProvidersRepository(AppDbContext context) : base(context)
        {
        }

        //get all services of a provider
        public async Task<List<Service>> GetServices(int providerId)
        {
            return await DbSet.Where(p => p.Id == providerId).SelectMany(p=>p.Services).ToListAsync();
        }

        public async Task<Provider?> GetByEmailAsync(string email)
        {
            return await DbSet.Where(p => string.Equals(p.User.Email, email)).FirstOrDefaultAsync();
        }

    }
    
}
