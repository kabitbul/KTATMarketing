using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
//using System.Web.Mvc; // או Microsoft.AspNetCore.Mvc ב-.NET Core


namespace KTSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AAmazonDashboardController : Controller
    {
        public ActionResult Index()
        {
            var viewModel = new AAmzMainDashboardVM();

            // 1. טעינת נתונים לגרף 60 יום אחרונים (דוגמה למילוי נתונים)
            var startDate60 = DateTime.Today.AddDays(-59);
            for (int i = 0; i < 60; i++)
            {
                var currentDate = startDate60.AddDays(i);
                viewModel.DailySalesLast60Days.Add(new DailySalesDataPoint
                {
                    DateLabel = currentDate.ToString("dd/MM"),
                    KtSales = GetSalesFromDb("KT", currentDate),
                    KesemSales = GetSalesFromDb("KESEM", currentDate),
                    GoralSales = GetSalesFromDb("GORAL", currentDate),
                    WebrushSales = GetSalesFromDb("WEBRUSH", currentDate)
                });
            }

            // 2. טעינת נתונים לגרף 18 חודשים אחרונים
            var startMonth18 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-17);
            for (int i = 0; i < 18; i++)
            {
                var currentMonth = startMonth18.AddMonths(i);
                
                viewModel.MonthlySalesLast18Months.MonthLabels.Add(currentMonth.ToString("MMM yyyy"));
                viewModel.MonthlySalesLast18Months.KtSales.Add(GetMonthlySalesFromDb("KT", currentMonth));
                viewModel.MonthlySalesLast18Months.KesemSales.Add(GetMonthlySalesFromDb("KESEM", currentMonth));
                viewModel.MonthlySalesLast18Months.GoralSales.Add(GetMonthlySalesFromDb("GORAL", currentMonth));
                viewModel.MonthlySalesLast18Months.WebrushSales.Add(GetMonthlySalesFromDb("WEBRUSH", currentMonth));
            }

            return View(viewModel);
        }

        // פונקציות עזר לדוגמה (להחלפה בשאילתות DB/EF שליפה מהמסד שלכם)
        private int GetSalesFromDb(string storeName, DateTime date)
        {
            // החלף בקריאה אמיתית ל-DB
            return 10; 
        }

        private int GetMonthlySalesFromDb(string storeName, DateTime month)
        {
            // החלף בקריאה אמיתית ל-DB
            return 300; 
        }
    }
}