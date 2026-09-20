using Dapper;
using KTSite.DataAccess.Data;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
public class AAmzRefundsController : Controller
{
 private readonly ApplicationDbContext _db;
public AAmzRefundsController(ApplicationDbContext db)
{
    _db = db;
}
    public IActionResult Index(DateTime? fromDate,DateTime? toDate,int? storeId,
    string? marketplace)
    {
DateTime maxDate = DateTime.Today.AddDays(-2);

DateTime selectedToDate =
    toDate.HasValue && toDate.Value.Date <= maxDate
        ? toDate.Value.Date
        : maxDate;

DateTime selectedFromDate =
    fromDate?.Date ?? selectedToDate.AddDays(-59);

        const string sql = @"
            SELECT
                (
                    SELECT ISNULL(SUM(Qty), 0)
FROM AAmzOrders
WHERE PurchaseDate >= @FromDate
  AND PurchaseDate < DATEADD(DAY, 1, @ToDate)
  AND (@StoreId IS NULL OR storeId = @StoreId)
  AND (@Marketplace IS NULL OR MarketPlace = @Marketplace)
                ) AS SoldUnits,

                (
                    SELECT ISNULL(SUM(Quantity), 0)
                    FROM AAmzRefunds
                    WHERE SaleDate >= @FromDate
                      AND SaleDate < DATEADD(DAY, 1, @ToDate)
                      AND (@StoreId IS NULL OR storeId = @StoreId)
                      AND (@Marketplace IS NULL OR MarketPlace = @Marketplace)
                ) AS RefundedUnits,

                (
                    SELECT ISNULL(SUM(NetRefundLoss), 0)
                    FROM AAmzRefunds
                    WHERE SaleDate >= @FromDate
                      AND SaleDate < DATEADD(DAY, 1, @ToDate)
                      AND (@StoreId IS NULL OR storeId = @StoreId)
                      AND (@Marketplace IS NULL OR MarketPlace = @Marketplace)
                ) AS NetRefundLoss";

        var connection = _db.Database. GetDbConnection();

AAmzRefundsVM vm = connection.QuerySingle<AAmzRefundsVM>(
    sql,
    new
    {
        FromDate = selectedFromDate,
        ToDate = selectedToDate,
        StoreId = storeId ,
        Marketplace = marketplace
    });

vm.FromDate = selectedFromDate;
vm.ToDate = selectedToDate;
vm.StoreId = storeId;
vm.Marketplace = marketplace;

vm.RefundRate =
    vm.SoldUnits > 0
        ? (decimal)vm.RefundedUnits / vm.SoldUnits * 100
        : 0;
const string storesSql = @"
WITH Sales AS
(
    SELECT
        storeId AS StoreId,
        SUM(Qty) AS SoldUnits
    FROM AAmzOrders
    WHERE PurchaseDate >= @FromDate
      AND PurchaseDate < DATEADD(DAY, 1, @ToDate)
      AND (@StoreId IS NULL OR storeId = @StoreId)
      AND (@Marketplace IS NULL OR MarketPlace = @Marketplace)
    GROUP BY storeId
),
Refunds AS
(
    SELECT
        StoreId,
        SUM(Quantity) AS RefundedUnits,
        SUM(NetRefundLoss) AS NetRefundLoss
    FROM AAmzRefunds
    WHERE SaleDate >= @FromDate
      AND SaleDate < DATEADD(DAY, 1, @ToDate)
      AND (@StoreId IS NULL OR StoreId = @StoreId)
      AND (@Marketplace IS NULL OR Marketplace = @Marketplace)
    GROUP BY StoreId
)
SELECT
    s.StoreId,

    CASE s.StoreId
        WHEN 1 THEN 'KT'
        WHEN 2 THEN 'Lital'
        WHEN 3 THEN 'Goral'
        WHEN 4 THEN 'Webrush'
    END AS StoreName,

    ISNULL(sa.SoldUnits, 0) AS SoldUnits,
    ISNULL(r.RefundedUnits, 0) AS RefundedUnits,

    CAST(
        CASE
            WHEN ISNULL(sa.SoldUnits, 0) > 0
                THEN ISNULL(r.RefundedUnits, 0) * 100.0 / sa.SoldUnits
            ELSE 0
        END
        AS decimal(18,2)
    ) AS RefundRate,

    ISNULL(r.NetRefundLoss, 0) AS NetRefundLoss

FROM (VALUES (1),(2),(3),(4)) s(StoreId)

LEFT JOIN Sales sa
    ON sa.StoreId = s.StoreId

LEFT JOIN Refunds r
    ON r.StoreId = s.StoreId

WHERE @StoreId IS NULL OR s.StoreId = @StoreId

ORDER BY s.StoreId;
";
vm.Stores = connection.Query<AAmzRefundStoreSummaryVM>(
    storesSql,
    new
    {
        FromDate = selectedFromDate,
        ToDate = selectedToDate,
        StoreId = storeId,
        Marketplace = marketplace
    }
).ToList();
const string marketplacesSql = @"
WITH Sales AS
(
    SELECT
        MarketPlace AS Marketplace,
        SUM(Qty) AS SoldUnits
    FROM AAmzOrders
    WHERE PurchaseDate >= @FromDate
      AND PurchaseDate < DATEADD(DAY, 1, @ToDate)
      AND (@StoreId IS NULL OR storeId = @StoreId)
      AND (@Marketplace IS NULL OR MarketPlace = @Marketplace)
    GROUP BY MarketPlace
),
Refunds AS
(
    SELECT
        Marketplace,
        SUM(Quantity) AS RefundedUnits,
        SUM(NetRefundLoss) AS NetRefundLoss
    FROM AAmzRefunds
    WHERE SaleDate >= @FromDate
      AND SaleDate < DATEADD(DAY, 1, @ToDate)
      AND (@StoreId IS NULL OR StoreId = @StoreId)
      AND (@Marketplace IS NULL OR Marketplace = @Marketplace)
    GROUP BY Marketplace
)
SELECT
    m.Marketplace,

    ISNULL(s.SoldUnits, 0) AS SoldUnits,
    ISNULL(r.RefundedUnits, 0) AS RefundedUnits,

    CAST(
        CASE
            WHEN ISNULL(s.SoldUnits, 0) > 0
                THEN ISNULL(r.RefundedUnits, 0) * 100.0 / s.SoldUnits
            ELSE 0
        END
        AS decimal(18,2)
    ) AS RefundRate,

    ISNULL(r.NetRefundLoss, 0) AS NetRefundLoss

FROM (VALUES ('US'),('CA')) m(Marketplace)

LEFT JOIN Sales s
    ON s.Marketplace = m.Marketplace

LEFT JOIN Refunds r
    ON r.Marketplace = m.Marketplace

WHERE @Marketplace IS NULL OR m.Marketplace = @Marketplace

ORDER BY
    CASE m.Marketplace
        WHEN 'US' THEN 1
        WHEN 'CA' THEN 2
        ELSE 3
    END;
";
vm.Marketplaces = connection.Query<AAmzRefundMarketplaceSummaryVM>(
    marketplacesSql,
    new
    {
        FromDate = selectedFromDate,
        ToDate = selectedToDate,
        StoreId = storeId,
        Marketplace = marketplace
    }
).ToList();
const string productsSql = @"
WITH Sales AS
(
    SELECT
        Asin,
        storeId AS StoreId,
        MarketPlace AS Marketplace,
        SUM(Qty) AS SoldUnits
    FROM AAmzOrders
    WHERE PurchaseDate >= @FromDate
      AND PurchaseDate < DATEADD(DAY, 1, @ToDate)
      AND (@StoreId IS NULL OR storeId = @StoreId)
      AND (@Marketplace IS NULL OR MarketPlace = @Marketplace)
    GROUP BY
        Asin,
        storeId,
        MarketPlace
),
Refunds AS
(
    SELECT
        Asin,
        StoreId,
        Marketplace,
        SUM(Quantity) AS RefundedUnits,
        SUM(NetRefundLoss) AS NetRefundLoss
    FROM AAmzRefunds
    WHERE SaleDate >= @FromDate
      AND SaleDate < DATEADD(DAY, 1, @ToDate)
      AND (@StoreId IS NULL OR StoreId = @StoreId)
      AND (@Marketplace IS NULL OR Marketplace = @Marketplace)
    GROUP BY
        Asin,
        StoreId,
        Marketplace
),
StoreTotals AS
(
    SELECT
        s.StoreId,
        SUM(s.SoldUnits) AS StoreSoldUnits,
        SUM(ISNULL(r.RefundedUnits, 0)) AS StoreRefundedUnits
    FROM Sales s

    LEFT JOIN Refunds r
        ON r.Asin = s.Asin
       AND r.StoreId = s.StoreId
       AND r.Marketplace = s.Marketplace

    GROUP BY
        s.StoreId
)

SELECT TOP 20
    s.Asin,
    s.StoreId,
    s.Marketplace,

    sk.ChinaName AS ProductName,
    sk.ImageUrl,

    s.SoldUnits,

    ISNULL(r.RefundedUnits, 0) AS RefundedUnits,

    CAST(
        ISNULL(r.RefundedUnits, 0) * 100.0 / s.SoldUnits
        AS decimal(18,2)
    ) AS RefundRate,

    ISNULL(r.NetRefundLoss, 0) AS NetRefundLoss,

    CAST(
        CASE
            WHEN st.StoreSoldUnits > 0
                THEN st.StoreRefundedUnits * 100.0 / st.StoreSoldUnits
            ELSE 0
        END
        AS decimal(18,2)
    ) AS StoreRefundRate,

    CASE
        WHEN st.StoreSoldUnits = 0 THEN 'Normal'

        WHEN
            (ISNULL(r.RefundedUnits, 0) * 100.0 / s.SoldUnits)
            >=
            ((st.StoreRefundedUnits * 100.0 / st.StoreSoldUnits) * 2)
            THEN 'Critical'

        WHEN
            (ISNULL(r.RefundedUnits, 0) * 100.0 / s.SoldUnits)
            >=
            ((st.StoreRefundedUnits * 100.0 / st.StoreSoldUnits) * 1.5)
            THEN 'High'

        ELSE 'Normal'
    END AS Severity

FROM Sales s

LEFT JOIN Refunds r
    ON r.Asin = s.Asin
   AND r.StoreId = s.StoreId
   AND r.Marketplace = s.Marketplace

LEFT JOIN StoreTotals st
    ON st.StoreId = s.StoreId

LEFT JOIN AAmzAsinToSku sk
    ON sk.Asin = s.Asin
   AND sk.StoreId = s.StoreId

WHERE s.SoldUnits >= 30
  AND ISNULL(r.RefundedUnits, 0) > 0

ORDER BY
    RefundRate DESC,
    RefundedUnits DESC;
";
vm.Products = connection.Query<AAmzRefundProductSummaryVM>(
    productsSql,
    new
    {
        FromDate = selectedFromDate,
        ToDate = selectedToDate,
        StoreId = storeId,
        Marketplace = marketplace
    }
).ToList();
return View(vm);
    }
public IActionResult Details(string asin, int storeId, string marketplace)
{
    var connection = _db.Database.GetDbConnection();

    string sql = @"
        SELECT
            r.OrderId,
            r.Asin,
            r.Quantity AS RefundedQty,
            r.SaleDate,
            r.RefundDate,
            r.NetRefundLoss,
            o.PurchaseDate,
            o.Qty AS OrderedQty
        FROM AAmzRefunds r
        LEFT JOIN AAmzOrders o
            ON o.AmazonOrdId = r.OrderId
            AND o.Asin = r.Asin
        WHERE r.Asin = @Asin
          AND r.StoreId = @StoreId
          AND r.Marketplace = @Marketplace
        ORDER BY r.RefundDate DESC";

    var vm = new AAmzRefundDetailsVM
    {
        Asin = asin,
        StoreId = storeId,
        Marketplace = marketplace,
        Refunds = connection.Query<AAmzRefundDetailRow>(
            sql,
            new
            {
                Asin = asin,
                StoreId = storeId,
                Marketplace = marketplace
            }).ToList()
    };

    return View(vm);
}
}
}