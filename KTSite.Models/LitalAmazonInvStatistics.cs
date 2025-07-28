using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class LitalAmazonInvStatistics
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Asin { get; set; }
        public string ChinaName{ get; set; }
        public string MarketPlace { get; set; }
        public int AmzAvailQty { get; set; }
        public int AmzInboundQty { get; set; }
        public int AmzAWDAvailQty { get; set; }
        public int AmzAWDInboundQty { get; set; }
        public int warehouseAvailQty { get; set; }
        public int warehouseOnTheWayQty { get; set; }
        public int avg3days { get; set; }
        public int avg14days { get; set; }
        public int avgMonth { get; set; }
        public int sales30Days { get; set; }
        public int avg3daysEbay { get; set; }
        public int daysToOOS { get; set; }
        public bool needToOrderFromChina { get; set; }
         public bool needToOSendFromWarehouse { get; set; }
        public bool restock { get; set; }
        public int onTheWay { get; set; }
        public bool restockNotDecided{ get; set; }

    }
}
