using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace РРО.Models
{
    public class Sell
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Manufacturer { get; set; }
        public string Product { get; set; }
        public int Count { get; set; }
        public int TotalSum { get; set; }
        public string Customer { get; set; }
        public int Discount { get; set; }

    }
}
