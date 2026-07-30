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
public List<DashboardRestockAlertVM> RestockAlerts { get; set; }
    = new List<DashboardRestockAlertVM>();
public List<DashboardFbaReceivingAlertVM> FbaReceivingAlerts { get; set; }
    = new List<DashboardFbaReceivingAlertVM>();
public List<DashboardTrendItem> MajorIncreases { get; set; } = new();
public List<DashboardTrendItem> MajorDecreases { get; set; } = new();
    }
public class DashboardTrendItem
{
    public string Store { get; set; } = "";
    public string Marketplace { get; set; } = "";
    public string Asin { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Title { get; set; } = "";

    public decimal Avg3Days { get; set; }
public decimal Avg14Days { get; set; }
public decimal Sales30Days { get; set; }
    public decimal Avg30Days { get; set; }
    public decimal ChangePercent { get; set; }

    public bool MajorIncrease { get; set; }
    public bool MajorDecrease { get; set; }
   public string GraphUrl { get; set; } = "";
public string? ImageUrl { get; set; }
}
public class DashboardRestockAlertVM
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = string.Empty;

    public string Marketplace { get; set; } = string.Empty;

    public string Asin { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int AvailableQty { get; set; }

    public int InboundQty { get; set; }

    public int AWDAvailableQty { get; set; }

    public int AWDInboundQty { get; set; }

    public int OnTheWayQty { get; set; }

    public int Average14Days { get; set; }

    public int DaysToOOS { get; set; }
public string GraphUrl { get; set; } = string.Empty;
public string? ImageUrl { get; set; }
}
public class DashboardFbaReceivingAlertVM
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public string StoreName { get; set; } = string.Empty;

    public string Marketplace { get; set; } = string.Empty;

    public string Asin { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int AvailableQty { get; set; }

    public int InboundShippedQty { get; set; }

    public int InboundReceivingQty { get; set; }

    public int ReservedQty { get; set; }

    public string DetectionReason { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
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