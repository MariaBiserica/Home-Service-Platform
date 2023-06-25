using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos
{
    public class ServiceForProviderDisplayDto
    {
        public string Title { get; set; }
        public int? ServiceTypeId { get; set; }
        public ServiceType? Type { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, float> Prices { get; set; }
        public int ProviderId { get; set; }
    }
}
