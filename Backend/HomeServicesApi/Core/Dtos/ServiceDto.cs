using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ServiceDto
    {
        public int ProviderId { get; set; }
        public string Title { get; set; }
        public int TypeId { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, float> Prices { get; set; }
    }
}
