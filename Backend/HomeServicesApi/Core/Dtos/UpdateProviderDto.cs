using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UpdateProviderDto
    {
        public string? Bio { get; set; }
        public Address? Address { get; set; }
    }
}
