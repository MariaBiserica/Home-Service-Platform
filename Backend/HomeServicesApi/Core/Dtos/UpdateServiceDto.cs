using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UpdateServiceDto
    {
        public int ServiceId { get; set; }
        public string? Title { get; set; }
        public int? TypeId { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, float>? Prices { get; set; }



    }
}
