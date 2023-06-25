using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class User : BaseEntity
    {
        public string? Username { get; set; }
        public string PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; } = false;
    }
}
