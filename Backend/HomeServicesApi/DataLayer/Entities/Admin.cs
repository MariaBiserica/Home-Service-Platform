using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DataLayer.Entities
{
    public class Admin : BaseEntity
    {
        public User User { get; set; }
    }
}
