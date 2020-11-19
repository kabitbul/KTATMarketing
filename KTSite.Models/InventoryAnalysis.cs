using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class InventoryAnalysis
    {
        [NotMapped]
        public int AvgDays { get; set; }
        [NotMapped]
        public int TimeToArrive { get; set; }
        [NotMapped]
        public int ProductId { get; set; }
        [NotMapped]
        public double MissingQuantity { get; set; }
        [NotMapped]
        public double Cost { get; set; }
        [NotMapped]
        public int InventoryCount { get; set; }
        [NotMapped]
        public int OnTheWay { get; set; }
        [NotMapped]
        public bool OwnByWarehouse { get; set; }
        [NotMapped]
        public double AvgSales { get; set; }

    }
}
