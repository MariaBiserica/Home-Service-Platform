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

        public async Task<List<Service>> GetServices(int providerId)
        {
            return await DbSet.Where(p => p.Id == providerId).SelectMany(p=>p.Services).ToListAsync();
        }

        public async Task<List<Service>> GetServicesByType(int providerId, ServiceType type)
        {
            return await DbSet.Where(p => p.Id == providerId).SelectMany(p => p.Services).Where(s=>s.Type.Name == type.Name).ToListAsync();
        }
        public async Task<Provider?> GetByEmailAsync(string email)
        {
            return await DbSet.Where(p => string.Equals(p.User.Email, email)).FirstOrDefaultAsync();
        }

        public async Task AddServiceAsync(int providerId, Service service)
        {
            var provider = await GetByIdAsync(providerId);
            provider.Services?.Add(service);
            await UpdateAsync(provider);
        }

    }
    
}
