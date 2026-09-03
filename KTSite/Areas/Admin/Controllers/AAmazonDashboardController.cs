using KTSite.DataAccess.Data;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KTSite.DataAccess.Repository.IRepository;
using Dapper;
using System.Data.Common;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmazonDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

    public AAmazonDashboardController(
        ApplicationDbContext db,
        IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }
private sealed class DashboardOrderRow
{
    public int StoreId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int Qty { get; set; }
}

        public async Task<IActionResult> Index(DateTime? fromDate,DateTime? toDate)
        {
            DateTime today = DateTime.Today;

           DateTime dailyEndDate = toDate?.Date ?? today;
           DateTime dailyStartDate =fromDate?.Date ?? dailyEndDate.AddDays(-59);
           if (dailyEndDate > today)
            {
              dailyEndDate = today;
             }
            if (dailyStartDate > dailyEndDate)
            {
              DateTime temp = dailyStartDate;
              dailyStartDate = dailyEndDate;
              dailyEndDate = temp;
             }
           const int maximumDailyRange = 366;

            if ((dailyEndDate - dailyStartDate).TotalDays + 1 > maximumDailyRange)
            {
                dailyStartDate = dailyEndDate.AddDays(-(maximumDailyRange - 1));
            }

if (dailyEndDate > today)
{
    dailyEndDate = today;
}
            DateTime currentMonthStart =
                new DateTime(today.Year, today.Month, 1);

            DateTime monthlyStartDate =
                currentMonthStart.AddMonths(-17);

            DateTime monthlyEndDate =
                currentMonthStart.AddMonths(1);
DateTime ordersStartDate =    dailyStartDate < monthlyStartDate        ? dailyStartDate        : monthlyStartDate;

DateTime dailyEndExclusive =
    dailyEndDate.AddDays(1);

DateTime ordersEndDate =    dailyEndExclusive > monthlyEndDate        ? dailyEndExclusive        : monthlyEndDate;
            AAmzMainDashboardVM viewModel = new AAmzMainDashboardVM
{
    GeneratedAt = DateTime.Now,
    DailyFromDate = dailyStartDate,
    DailyToDate = dailyEndDate
};
            /*
             * Store table
             */
            List<AAmazonStores> stores = await _db.AAmazonStores
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

List<DashboardOrderRow> orders = await _db.AAmzOrders
    .AsNoTracking()
    .Where(x =>
        x.PurchaseDate >= ordersStartDate &&
        x.PurchaseDate < ordersEndDate)
    .Select(x => new DashboardOrderRow
    {
        StoreId = x.storeId,
        PurchaseDate = x.PurchaseDate,
        Qty = x.Qty
    })
    .ToListAsync();

            /*
             * Latest inventory cost record for each Store + Marketplace.
             *
             * A store may have separate US and CA cost records.
             * We take the latest record for each marketplace and then
             * sum all marketplaces belonging to that store.
             */
            List<AAmzInventoryCost> inventoryCostRecords =
                await _db.AAmzInventoryCost
                    .AsNoTracking()
                    .ToListAsync();

            Dictionary<int, decimal> inventoryCostByStore =
                inventoryCostRecords
                    .GroupBy(x => new
                    {
                        x.StoreId,
                        MarketPlace = (x.MarketPlace ?? string.Empty)
                            .Trim()
                            .ToUpper()
                    })
                    .Select(group => group
                        .OrderByDescending(x => x.DateCreated)
                        .ThenByDescending(x => x.Id)
                        .First())
                    .GroupBy(x => x.StoreId)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Sum(x =>
                            Convert.ToDecimal(x.FBACost) +
                            Convert.ToDecimal(x.AWDCost) +
                            Convert.ToDecimal(x.OnTheWayCost))
                    );

            /*
             * Dashboard store summary
             */
            foreach (AAmazonStores store in stores)
            {
                decimal inventoryCost =
                    inventoryCostByStore.TryGetValue(
                        store.Id,
                        out decimal storeInventoryCost)
                        ? storeInventoryCost
                        : 0m;

                int unitsLast60Days = orders
                    .Where(x =>
                        x.StoreId == store.Id &&
                        x.PurchaseDate >= dailyStartDate && x.PurchaseDate < dailyEndDate.AddDays(1))
                    .Sum(x => x.Qty);

                viewModel.Stores.Add(new AmazonStoreDashboardVM
                {
                    StoreId = store.Id,
                    StoreName = GetDisplayStoreName(store.StoreName),
                    InventoryCost = inventoryCost,
                    UnitsLast60Days = unitsLast60Days
                });
            }

            viewModel.TotalInventoryCost =
                viewModel.Stores.Sum(x => x.InventoryCost);

            foreach (AmazonStoreDashboardVM store in viewModel.Stores)
            {
                store.InventorySharePercent =
                    viewModel.TotalInventoryCost > 0
                        ? Math.Round(
                            store.InventoryCost /
                            viewModel.TotalInventoryCost * 100m,
                            1)
                        : 0m;
            }

            BuildDailyChart(viewModel,stores,orders, dailyStartDate,dailyEndDate);

            BuildMonthlyChart(
                viewModel,
                stores,
                orders,
                monthlyStartDate);
viewModel.RestockAlerts = GetRestockAlerts();

FillMajorTrends(viewModel);
viewModel.FbaReceivingAlerts =
    await GetFbaReceivingAlertsAsync();
viewModel.MissingTrackingAlerts =
    await GetMissingTrackingAlertsAsync();
viewModel.NewProducts = await GetNewProductsAsync();
return View(viewModel);
         
        }
public async Task<List<DashboardNewProductVM>> GetNewProductsAsync()
{
    const string sql = @"
WITH PurchaseSummary AS
(
    SELECT
        purchase.StoreId,
        purchase.MarketPlace AS Marketplace,
        purchase.ProductAsin AS Asin,
        SUM(purchase.Quantity) AS PurchasedQty,
        MIN(purchase.DateOrdered) AS FirstPurchaseDate,
        MAX(NULLIF(purchase.ProductChinaName, '')) AS PurchaseProductName
    FROM dbo.AAmzStockPurchase purchase
    GROUP BY
        purchase.StoreId,
        purchase.MarketPlace,
        purchase.ProductAsin
)
SELECT
    summary.StoreId,
    CASE
        WHEN store.StoreName = 'LITAL' THEN 'KESEM'
        ELSE store.StoreName
    END AS StoreName,
    summary.Marketplace,
    summary.Asin,
    COALESCE(NULLIF(product.ChinaName, ''), summary.PurchaseProductName, '') AS ProductName,
    product.ImageUrl,
    summary.PurchasedQty,
    summary.FirstPurchaseDate,
    DATEDIFF(DAY, summary.FirstPurchaseDate, GETDATE()) AS DaysSinceFirstPurchase
FROM PurchaseSummary summary
INNER JOIN dbo.AAmazonStores store
    ON store.Id = summary.StoreId
OUTER APPLY
(
    SELECT TOP 1
        sku.ChinaName,
        sku.ImageUrl
    FROM dbo.AAmzAsinToSku sku
    WHERE sku.StoreId = summary.StoreId
      AND sku.Asin = summary.Asin
) product
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.AAmzOrders orders
    WHERE orders.storeId = summary.StoreId
      AND orders.MarketPlace = summary.Marketplace
      AND orders.Asin = summary.Asin
)
ORDER BY
    summary.FirstPurchaseDate ASC,
    summary.StoreId,
    summary.Marketplace,
    summary.Asin;";

    var connection = _db.Database.GetDbConnection();

    var items = await connection.QueryAsync<DashboardNewProductVM>(sql);

    return items.ToList();
}
private void FillMajorTrends(AAmzMainDashboardVM viewModel)
{
    var allTrendItems = new List<DashboardTrendItem>();

    List<AAmazonStores> stores =
        _unitOfWork.AAmazonStores.GetList();

    string[] marketplaces =
    {
        SD.marketPlaceUS,
        SD.marketPlaceCA
    };

    foreach (AAmazonStores store in stores)
    {
        foreach (string marketplace in marketplaces)
        {
            AddStoreTrends(
                allTrendItems,
                store,
                marketplace);
        }
    }

    viewModel.MajorIncreases = allTrendItems
        .Where(x =>
            x.MajorIncrease &&
            !x.MajorDecrease)
        .OrderByDescending(GetIncreaseStrength)
        .Take(10)
        .ToList();

    viewModel.MajorDecreases = allTrendItems
        .Where(x =>
            x.MajorDecrease &&
            !x.MajorIncrease)
        .OrderByDescending(GetDecreaseStrength)
        .Take(10)
        .ToList();
}
private void AddStoreTrends(
    List<DashboardTrendItem> target,
    AAmazonStores store,
    string marketplace)
{
    var inventory =
        _unitOfWork.AAmzFBAInventory.inventoryIndexData(
            false,
            marketplace,
            store.Id);

    string controllerName =
        $"AAmzAsinToSku{store.StoreName}";

    string actionName =
        marketplace == SD.marketPlaceUS
            ? "GraphUS2months"
            : "GraphCA2months";

    target.AddRange(
        inventory
            .Where(x =>
                x.HasSalesHistoryOver30Days &&
                x.sales30Days > 10 &&
               // x.avgMonth > 0 &&
                x.AmzAvailQty >= 60 &&
                (x.majorIncrease || x.majorDecrease))
            .Select(x => new DashboardTrendItem
            {
                Store = GetDisplayStoreName(store.StoreName),
                Marketplace = marketplace,

                Asin = x.Asin ?? "",
                Sku = x.sku ?? "",
                Title = x.ChinaName ?? "",
                 ImageUrl = x.ImageUrl,

                Avg3Days = x.avg3days,
                Avg14Days = x.avg14days,
                Avg30Days = x.avgMonth,
                Sales30Days = x.sales30Days,

                MajorIncrease = x.majorIncrease,
                MajorDecrease = x.majorDecrease,

                GraphUrl = Url.Action(
                    actionName,
                    controllerName,
                    new { id = x.Id }) ?? ""
            }));
}
//private static decimal GetIncreaseStrength(DashboardTrendItem item)
//{
//    if (item.Avg14Days <= 0 || item.Avg30Days <= 0)
//        return 0;

//    decimal shortTermIncrease =
//        (decimal)item.Avg3Days / item.Avg14Days;

//    decimal mediumTermIncrease =
//        (decimal)item.Avg14Days / item.Avg30Days;

//    return shortTermIncrease * mediumTermIncrease;
//}
private static decimal GetIncreaseStrength(DashboardTrendItem item)
{
    if (item.Sales30Days <= 0 || item.Avg3Days <= 0)
        return 0;

    decimal avg30Exact =
        (decimal)item.Sales30Days / 30m;

    return (decimal)item.Avg3Days / avg30Exact;
}

private static decimal GetDecreaseStrength(DashboardTrendItem item)
{
    if (item.Avg3Days <= 0 || item.Avg14Days <= 0)
        return decimal.MaxValue;

    decimal shortTermDecrease =
        (decimal)item.Avg14Days / item.Avg3Days;

    decimal mediumTermDecrease =
        item.Avg30Days > 0
            ? (decimal)item.Avg30Days / item.Avg14Days
            : 1;

    return shortTermDecrease * mediumTermDecrease;
}
private List<DashboardRestockAlertVM> GetRestockAlerts()
{
    var alerts = new List<DashboardRestockAlertVM>();

    AddRestockAlerts(
        alerts,
        storeId: SD.KTStoreId,
        storeName: "KT",
        marketplace: SD.marketPlaceUS);

    AddRestockAlerts(
        alerts,
        storeId: SD.KTStoreId,
        storeName: "KT",
        marketplace: SD.marketPlaceCA);

    AddRestockAlerts(
        alerts,
        storeId: SD.LitalStoreId,
        storeName: "KESEM",
        marketplace: SD.marketPlaceUS);

    AddRestockAlerts(
        alerts,
        storeId: SD.GoralStoreId,
        storeName: "GORAL",
        marketplace: SD.marketPlaceUS);

    AddRestockAlerts(
        alerts,
        storeId: SD.WebrushStoreId,
        storeName: "WEBRUSH",
        marketplace: SD.marketPlaceUS);

    return alerts
        .OrderBy(x => x.StoreName)
        .ThenBy(x => x.Marketplace)
        .ThenBy(x => x.DaysToOOS)
        .ToList();
}

private void AddRestockAlerts(
    List<DashboardRestockAlertVM> alerts,
    int storeId,
    string storeName,
    string marketplace)
{
string controllerName = storeId switch
{
    SD.KTStoreId => "AAmzAsinToSkuKT",
    SD.LitalStoreId => "AAmzAsinToSkuLital",
    SD.GoralStoreId => "AAmzAsinToSkuGoral",
    SD.WebrushStoreId => "AAmzAsinToSkuWebrush",
    _ => string.Empty
};

string actionName = marketplace == SD.marketPlaceUS
    ? "GraphUS2months"
    : "GraphCA2months";
    List<AmazonInvStatistics> inventoryRows =
        _unitOfWork.AAmzFBAInventory.inventoryIndexData(
            showRestock: true,
            marketplace: marketplace,
            storeId: storeId);

    foreach (AmazonInvStatistics item in inventoryRows)
    {
      bool restockNotDecided =   marketplace == SD.marketPlaceCA
                                 ? item.restockNotDecidedCA
                                 : item.restockNotDecided;
       if (!item.needToOrderFromChina || restockNotDecided)
        {
            continue;
        }

        alerts.Add(new DashboardRestockAlertVM
        {
            StoreId = storeId,
            StoreName = storeName,
            Marketplace = marketplace,
            Asin = item.Asin ?? string.Empty,
            ProductName = item.ChinaName ?? string.Empty,
            AvailableQty = item.AmzAvailQty,
            InboundQty = item.AmzInboundQty,
            AWDAvailableQty = item.AmzAWDAvailQty,
            AWDInboundQty = item.AmzAWDInboundQty,
            OnTheWayQty = item.onTheWay,
            Average14Days = item.avg14days,
            DaysToOOS = item.daysToOOS,
            
GraphUrl = string.IsNullOrWhiteSpace(controllerName)
    ? string.Empty
    : Url.Action(
        actionName,
        controllerName,
        new { id = item.Id }) ?? string.Empty,
       ImageUrl = item.ImageUrl,
        });
    }
}
        private static void BuildDailyChart(
            AAmzMainDashboardVM viewModel,
            List<AAmazonStores> stores,
            List<DashboardOrderRow> orders,
            DateTime startDate,
            DateTime endDate)
        {
            List<DateTime> dates = Enumerable
                .Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .ToList();

            viewModel.DailyLabels = dates
                .Select(date => date.ToString("dd/MM"))
                .ToList();

            foreach (AAmazonStores store in stores)
            {
                DashboardChartSeriesVM series =
                    new DashboardChartSeriesVM
                    {
                        StoreId = store.Id,
                        StoreName =
                            GetDisplayStoreName(store.StoreName)
                    };

                foreach (DateTime date in dates)
                {
                    int quantity = orders
                        .Where(x =>
                            x.StoreId == store.Id &&
                            x.PurchaseDate.Date == date.Date)
                        .Sum(x => (int)x.Qty);

                    series.Values.Add(quantity);
                }

                viewModel.DailySeries.Add(series);
            }
        }

        private static void BuildMonthlyChart(
            AAmzMainDashboardVM viewModel,
            List<AAmazonStores> stores,
            List<DashboardOrderRow> orders,
            DateTime startMonth)
        {
            List<DateTime> months = Enumerable
                .Range(0, 18)
                .Select(offset => startMonth.AddMonths(offset))
                .ToList();

            viewModel.MonthlyLabels = months
                .Select(month => month.ToString("MMM yyyy"))
                .ToList();

            foreach (AAmazonStores store in stores)
            {
                DashboardChartSeriesVM series =
                    new DashboardChartSeriesVM
                    {
                        StoreId = store.Id,
                        StoreName =
                            GetDisplayStoreName(store.StoreName)
                    };

                foreach (DateTime month in months)
                {
                    DateTime nextMonth = month.AddMonths(1);

                    int quantity = orders
                        .Where(x =>
                            x.StoreId == store.Id &&
                            x.PurchaseDate >= month &&
                            x.PurchaseDate < nextMonth)
                        .Sum(x => (int)x.Qty);

                    series.Values.Add(quantity);
                }

                viewModel.MonthlySeries.Add(series);
            }
        }

        private static string GetDisplayStoreName(string storeName)
        {
            if (string.IsNullOrWhiteSpace(storeName))
            {
                return "Unknown";
            }

            string normalizedName =
                storeName.Trim().ToUpperInvariant();

            // The existing project uses "Lital" for the KESEM account.
            if (normalizedName.Contains("LITAL") ||
                normalizedName.Contains("KESEM"))
            {
                return "KESEM";
            }

            if (normalizedName.Contains("GORAL"))
            {
                return "GORAL";
            }

            if (normalizedName.Contains("WEBRUSH") ||
                normalizedName.Contains("WEB BRUSH"))
            {
                return "WEBRUSH";
            }

            if (normalizedName.Contains("KT"))
            {
                return "KT";
            }

            return storeName.Trim();
        }
public async Task<List<DashboardFbaReceivingAlertVM>>
    GetFbaReceivingAlertsAsync()
{
    const string sql = @"
        SELECT
            alert.Id,
            alert.StoreId,
            alert.Marketplace,
            alert.Asin,

            CASE
                WHEN store.StoreName = 'LITAL' THEN 'KESEM'
                ELSE store.StoreName
            END AS StoreName,

            ISNULL(product.ChinaName, '') AS ProductName,
            product.ImageUrl,
 
            alert.PreviousAvailableQty,
            alert.PreviousInboundShippedQty,
            alert.PreviousInboundReceivingQty,
            alert.PreviousReservedQty,
            alert.AvailableQty,
            alert.InboundShippedQty,
            alert.InboundReceivingQty,
            alert.ReservedQty,
            alert.DetectionReason,
            alert.CreatedDate

        FROM dbo.AAmzFBAReceivingAlerts alert

        LEFT JOIN dbo.AAmzAsinToSku product
            ON product.StoreId = alert.StoreId
           AND product.Asin = alert.Asin

        LEFT JOIN dbo.AAmazonStores store
            ON store.Id = alert.StoreId

        WHERE alert.IsHandled = 0

        ORDER BY
            alert.CreatedDate DESC,
            alert.StoreId,
            alert.Marketplace,
            alert.Asin;";

    DbConnection connection = _db.Database.GetDbConnection();

    IEnumerable<DashboardFbaReceivingAlertVM> alerts =
        await connection.QueryAsync<DashboardFbaReceivingAlertVM>(sql);

    return alerts.ToList();
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MarkFbaReceivingAlertHandled(int id)
{
    const string sql = @"
        UPDATE dbo.AAmzFBAReceivingAlerts
        SET
            IsHandled = 1,
            HandledDate = CAST(
                SYSUTCDATETIME()
                AT TIME ZONE 'UTC'
                AT TIME ZONE 'Israel Standard Time'
                AS datetime2
            )
        WHERE Id = @Id
          AND IsHandled = 0;";

    var connection = _db.Database.GetDbConnection();

    await connection.ExecuteAsync(
        sql,
        new { Id = id });

    return RedirectToAction(nameof(Index));
}
public async Task<List<DashboardMissingTrackingAlertVM>>
    GetMissingTrackingAlertsAsync()
{
    const string sql = @"
        SELECT
            purchase.Id,
            purchase.StoreId,
            store.StoreName,
            purchase.MarketPlace AS Marketplace,
            purchase.ProductAsin AS Asin,
            purchase.ProductChinaName AS ProductName,
            purchase.Quantity,
            purchase.DateOrdered,
            DATEDIFF(DAY, purchase.DateOrdered, GETDATE()) AS DaysWaiting,
            purchase.lineNumber AS LineNumber
        FROM dbo.AAmzStockPurchase purchase
        INNER JOIN dbo.AAmazonStores store
            ON store.Id = purchase.StoreId
        WHERE purchase.DateReceived = '0001-01-01'
          AND purchase.InboundUpdated = 0
          AND purchase.DateOrdered < DATEADD(DAY, -30, GETDATE())
        ORDER BY purchase.DateOrdered ASC;";

    var connection = _db.Database.GetDbConnection();

    var alerts = await connection.QueryAsync<DashboardMissingTrackingAlertVM>(
        sql);

    return alerts.ToList();
}
    }
}