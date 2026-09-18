using System;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class AAmzOrderSearchVM
    {
        // Filters
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string? SearchText { get; set; }
        public string? Sku { get; set; }
        public string? AmazonOrderId { get; set; }

        public int? StoreId { get; set; }
        public string? Marketplace { get; set; }

        // Summary
        public int TotalOrders { get; set; }
        public int TotalUnits { get; set; }
        public int TotalRows { get; set; }

        public bool IsLimited => TotalRows > 1000;

        // Results
        public List<AAmzOrderSearchRow> Orders { get; set; } = new();
    }


    public class AAmzOrderSearchRow
{
    public DateTime PurchaseDate { get; set; }

    public int StoreId { get; set; }

    public string? Asin { get; set; }

    public int Quantity { get; set; }

    public string? ChinaName { get; set; }

    public string? ImageUrl { get; set; }
}
}