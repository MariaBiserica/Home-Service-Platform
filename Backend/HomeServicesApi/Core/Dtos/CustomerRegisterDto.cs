using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class CustomerRegisterDto
    {
        public UserRegisterDto UserData { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
