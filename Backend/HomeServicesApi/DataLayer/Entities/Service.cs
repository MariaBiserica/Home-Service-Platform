using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Service : BaseEntity
    {
        public string Title { get; set; } 
        public ServiceType Type { get; set; }
        public string Description { get; set; }
        public Dictionary<string, float> Prices { get; set; }
        public Provider Provider { get; set; }
        
    }
}
