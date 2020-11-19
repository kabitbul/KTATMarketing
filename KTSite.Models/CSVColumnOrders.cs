using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KTSite.Models
{
    public class CSVColumnOrders
    {
        public string Product { get; set; }
        
        public string Empty { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Quantity { get; set; }
        public string Weight { get; set; }

    }
}
