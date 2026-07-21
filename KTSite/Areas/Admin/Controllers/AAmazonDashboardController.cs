using KTSite.DataAccess.Data;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmazonDashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AAmazonDashboardController(ApplicationDbContext db)
        {
            _db = db;
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

            return View(viewModel);
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
    }
}