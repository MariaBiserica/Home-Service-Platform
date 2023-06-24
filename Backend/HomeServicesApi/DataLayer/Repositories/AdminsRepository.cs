using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public class AdminsRepository : RepositoryBase<Admin>
    {
        public AdminsRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await DbSet.Where(a => string.Equals(a.User.Email, email)).FirstOrDefaultAsync();
        }
    }
}
