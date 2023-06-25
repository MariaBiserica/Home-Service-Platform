using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataLayer.Entities
{
    public class Service : BaseEntity
    {
        public string Title { get; set; }
        public int? ServiceTypeId { get; set; }
        public ServiceType? Type { get; set; }
        public string? Description { get; set; }
        [Column(TypeName = "nvarchar(max)")] 
        public string PricesData { get; set; }

        [NotMapped]
        public Dictionary<string, float> Prices
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, float>>(PricesData);
            set => PricesData = JsonConvert.SerializeObject(value);
        }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public List<Booking> Bookings { get; set; }

    }
}
