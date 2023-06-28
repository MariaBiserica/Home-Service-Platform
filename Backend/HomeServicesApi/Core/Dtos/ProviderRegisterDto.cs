using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ProviderRegisterDto
    {
        public UserRegisterDto UserData { get; set; }
        public string Bio { get; set; }

    }
}
