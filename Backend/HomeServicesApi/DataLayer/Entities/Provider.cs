﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Provider : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string Bio { get; set; }
        public byte Rating { get; set; } = 0;
        public int? AddressId { get; set; }
        public Address? Address { get; set; }
        public List<Service>? Services { get; set; }
    }
}
