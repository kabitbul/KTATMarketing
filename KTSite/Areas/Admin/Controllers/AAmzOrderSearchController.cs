using Dapper;
using KTSite.DataAccess.Data;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmzOrderSearchController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AAmzOrderSearchController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index(
            DateTime? fromDate,
            DateTime? toDate,
            string? searchText,
            int? storeId)
        {
            DateTime today = DateTime.Today;


            // Default = last 7 days including today
            DateTime searchToDate =
                toDate?.Date ?? today;

            DateTime searchFromDate =
                fromDate?.Date ?? searchToDate.AddDays(-6);


            // Do not allow future end date
            if (searchToDate > today)
            {
                searchToDate = today;
            }


            // If dates entered backwards - switch them
            if (searchFromDate > searchToDate)
            {
                DateTime temp = searchFromDate;

                searchFromDate = searchToDate;
                searchToDate = temp;
            }


            // Clean ASIN
            searchText = string.IsNullOrWhiteSpace(searchText)? null:searchText.Trim();


            AAmzOrderSearchVM viewModel =
    new AAmzOrderSearchVM
    {
        FromDate = searchFromDate,
        ToDate = searchToDate,

        SearchText = searchText,

        StoreId = storeId
    };


            /*
             * ToDate on the screen is inclusive.
             *
             * For example:
             *
             * From: 16/09/2026
             * To:   16/09/2026
             *
             * SQL:
             *
             * PurchaseDate >= 16/09 00:00
             * PurchaseDate <  17/09 00:00
             */
            DateTime toDateExclusive =
                searchToDate.AddDays(1);


       const string whereSql = @"
WHERE
        o.PurchaseDate >= @FromDate
    AND o.PurchaseDate < @ToDateExclusive

    AND
    (
        @SearchText IS NULL
        OR o.Asin = @SearchText
        OR sk.ChinaName LIKE '%' + @SearchText + '%'
    )

    AND
    (
        @StoreId IS NULL
        OR o.storeId = @StoreId
    )
";


string summarySql = @"
SELECT
    COUNT(*) AS TotalRows,
    ISNULL(SUM(o.Qty), 0) AS TotalUnits

FROM AAmzOrders o

OUTER APPLY
(
    SELECT TOP 1
        p.ChinaName,
        p.ImageUrl

    FROM AAmzAsinToSku p

    WHERE
        p.Asin = o.Asin
        AND p.StoreId = o.storeId
) sk

"
+ whereSql;


string ordersSql = @"
SELECT TOP 1000

    o.PurchaseDate,

    o.storeId AS StoreId,

    o.Asin,

    o.Qty AS Quantity,

    sk.ChinaName,

    sk.ImageUrl

FROM AAmzOrders o

OUTER APPLY
(
    SELECT TOP 1
        p.ChinaName,
        p.ImageUrl

    FROM AAmzAsinToSku p

    WHERE
        p.Asin = o.Asin
        AND p.StoreId = o.storeId
) sk

"
+ whereSql +
@"
ORDER BY
    o.PurchaseDate DESC
";


var parameters = new
{
    FromDate = searchFromDate,

    ToDateExclusive = toDateExclusive,

    SearchText = searchText,

    StoreId = storeId
};


            var connection =
                _db.Database.GetDbConnection();


            /*
             * Summary
             */
            AAmzOrderSearchSummary summary =
                await connection
                    .QueryFirstAsync<AAmzOrderSearchSummary>(
                        summarySql,
                        parameters);


            viewModel.TotalRows =
                summary.TotalRows;

            viewModel.TotalUnits =
                summary.TotalUnits;


            /*
             * Results
             */
            var orders =
                await connection
                    .QueryAsync<AAmzOrderSearchRow>(
                        ordersSql,
                        parameters);


            viewModel.Orders =
                orders.ToList();


            return View(viewModel);
        }


        private sealed class AAmzOrderSearchSummary
        {
            public int TotalRows { get; set; }

            public int TotalUnits { get; set; }
        }
    }
}