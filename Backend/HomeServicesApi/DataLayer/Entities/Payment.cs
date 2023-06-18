using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Payment : BaseEntity
    {
        public float Amount { get; set; }
        public DateTime DateProcessed { get; set; }
        public bool IsProcessed { get; set; }
    }
}
