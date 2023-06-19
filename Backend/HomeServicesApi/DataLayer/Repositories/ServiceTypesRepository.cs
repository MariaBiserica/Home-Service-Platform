using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Repositories
{
    public class ServiceTypesRepository : RepositoryBase<ServiceType>
    {
        public ServiceTypesRepository(AppDbContext context) : base(context)
        {
        }


    }
}
