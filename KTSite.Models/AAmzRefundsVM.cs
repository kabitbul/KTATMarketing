using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KTSite.Models
{
    public class AAmzRefundsVM
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public int SoldUnits { get; set; }
    public int RefundedUnits { get; set; }

    public decimal RefundRate { get; set; }
    public decimal NetRefundLoss { get; set; }
public int? StoreId { get; set; }
public string? Marketplace { get; set; }
public List<AAmzRefundStoreSummaryVM> Stores { get; set; } = new();
public List<AAmzRefundMarketplaceSummaryVM> Marketplaces { get; set; } = new();
public List<AAmzRefundProductSummaryVM> Products { get; set; } = new();
}  
public class AAmzRefundStoreSummaryVM
{
    public int StoreId { get; set; }
    public string StoreName { get; set; }

    public int SoldUnits { get; set; }
    public int RefundedUnits { get; set; }

    public decimal RefundRate { get; set; }
    public decimal NetRefundLoss { get; set; }
}
public class AAmzRefundMarketplaceSummaryVM
{
    public string Marketplace { get; set; }

    public int SoldUnits { get; set; }
    public int RefundedUnits { get; set; }

    public decimal RefundRate { get; set; }
    public decimal NetRefundLoss { get; set; }
}
public class AAmzRefundProductSummaryVM
{
    public string Asin { get; set; }
public string ProductName { get; set; }
public string ImageUrl { get; set; }
    public int StoreId { get; set; }
    public string Marketplace { get; set; }

    public int SoldUnits { get; set; }
    public int RefundedUnits { get; set; }

    public decimal RefundRate { get; set; }
    public decimal NetRefundLoss { get; set; }
public decimal StoreRefundRate { get; set; }
public string Severity { get; set; }
}
}
