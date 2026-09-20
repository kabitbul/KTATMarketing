using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace KTSite.Models
{
    public class AAmzRefundDetailsVM
{
    public string Asin { get; set; }
    public string ChinaName { get; set; }
    public int StoreId { get; set; }
    public string Marketplace { get; set; }

    public List<AAmzRefundDetailRow> Refunds { get; set; } = new();

    public int TotalRefundedUnits =>
        Refunds.Sum(x => x.RefundedQty);

    public int RefundEvents =>
        Refunds.Count;

    public int MaxRefundQty =>
        Refunds.Any() ? Refunds.Max(x => x.RefundedQty) : 0;

    public decimal TotalRefundLoss =>
        Refunds.Sum(x => x.NetRefundLoss);
}
public class AAmzRefundDetailRow
{
    public string OrderId { get; set; }

    public string Asin { get; set; }

    public int RefundedQty { get; set; }

    public int? OrderedQty { get; set; }

    public DateTime? PurchaseDate { get; set; }

    public DateTime? SaleDate { get; set; }

    public DateTime RefundDate { get; set; }

    public int? DaysUntilRefund { get; set; }

    public decimal NetRefundLoss { get; set; }

    public int StoreId { get; set; }

    public string Marketplace { get; set; }
}
}
