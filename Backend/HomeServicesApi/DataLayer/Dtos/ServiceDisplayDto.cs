using DataLayer.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos
{
    public class ServiceDisplayDto
    {
        public string Title { get; set; }
        public int? ServiceTypeId { get; set; }
        public ServiceType? Type { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, float> Prices { get; set; }
        public int ProviderId { get; set; }
        public ProviderDisplayDto? Provider { get; set; }
    }
}
