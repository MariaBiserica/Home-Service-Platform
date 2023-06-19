using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Repositories
{
    public class AdminsRepository : RepositoryBase<Admin>
    {
        public AdminsRepository(AppDbContext context) : base(context)
        {
        }
    }
}
