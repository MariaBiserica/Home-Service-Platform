using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayer.Repositories
{
    public class PaymentRepository : RepositoryBase<Payment>
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
