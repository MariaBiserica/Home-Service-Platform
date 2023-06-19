using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class AddressRepository : RepositoryBase<Address>
    {
        public AddressRepository(AppDbContext context) : base(context)
        {
        }
    }
}
