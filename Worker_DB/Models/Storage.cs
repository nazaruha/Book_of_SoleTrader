using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker_DB.Models
{
    public class Storage
    {
        public int ManufacturerId { get; set; }
        public int ProductId { get; set; }
        public int Price { get; set; }
        public int Count { get; set; }
    }
}
