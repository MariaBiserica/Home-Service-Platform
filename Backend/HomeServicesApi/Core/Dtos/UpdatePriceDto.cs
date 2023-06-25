using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UpdatePriceDto
    {
        public int ServiceId { get; set; }
        public string PriceKey { get; set; }
        public float PriceValue { get; set; }
    }
}
