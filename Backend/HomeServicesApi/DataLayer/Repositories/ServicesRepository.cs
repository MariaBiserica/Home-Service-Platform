using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class ServicesRepository : RepositoryBase<Service>
    {
        public ServicesRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Service>> GetServicesByType(int typeId)
        {
            return await DbSet.Where(s => s.ServiceTypeId == typeId).ToListAsync();
        }

        public async Task<List<Service>> GetServicesByProvider(int providerId)
        {
            return await DbSet.Where(s => s.ProviderId == providerId).ToListAsync();
        }


    }
}
