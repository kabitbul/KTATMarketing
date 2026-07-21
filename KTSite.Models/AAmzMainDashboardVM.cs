using System;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class AAmzMainDashboardVM
    {
        public DateTime GeneratedAt { get; set; }
        public DateTime DailyFromDate { get; set; }

    public DateTime DailyToDate { get; set; }

        // Inventory value
        public decimal TotalInventoryCost { get; set; }

        public List<AmazonStoreDashboardVM> Stores { get; set; }
            = new List<AmazonStoreDashboardVM>();

        // Daily units sold - last 60 days
        public List<string> DailyLabels { get; set; }
            = new List<string>();

        public List<DashboardChartSeriesVM> DailySeries { get; set; }
            = new List<DashboardChartSeriesVM>();

        // Monthly units sold - last 18 months
        public List<string> MonthlyLabels { get; set; }
            = new List<string>();

        public List<DashboardChartSeriesVM> MonthlySeries { get; set; }
            = new List<DashboardChartSeriesVM>();
    }

    public class AmazonStoreDashboardVM
    {
        public int StoreId { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public decimal InventoryCost { get; set; }

        public int UnitsLast60Days { get; set; }

        public decimal InventorySharePercent { get; set; }
    }

    public class DashboardChartSeriesVM
    {
        public int StoreId { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public List<int> Values { get; set; }
            = new List<int>();
    }
}