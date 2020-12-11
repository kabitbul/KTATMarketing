using KTSite.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class ProductInventory
    {
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Inventory { get; set; }
        public int OnTheWay { get; set; }
        public double Cost { get; set; }
        public string OwnByWarehouse { get; set; }
        public string Restock { get; set; }
        public int TotalSales3 { get; set; }
        public int TotalSales7 { get; set; }
        public double DailyAvg3 { get; set; }
        public double DailyAvg7 { get; set; }
        public double DaysToOOS { get; set; }
        public string DaysToOOSstr { get; set; }
        public string DailyAvg7str { get; set; }
        public string DailyAvg3str { get; set; }
    }
}
