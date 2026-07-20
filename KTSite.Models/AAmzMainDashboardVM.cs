using System;
using System.Collections.Generic;

namespace KTSite.Models
{
    // ViewModel ראשי של הדאשבורד
    public class AAmzMainDashboardVM
    {
        // גרף 1: מכירות יומיות ל-60 יום האחרונים
        public List<DailySalesDataPoint> DailySalesLast60Days { get; set; } = new List<DailySalesDataPoint>();

        // גרף 2: מכירות חודשיות מצטברות ל-18 חודשים
        public StackedMonthlySalesVM MonthlySalesLast18Months { get; set; } = new StackedMonthlySalesVM();
    }

    // נתונים יומיים לגרף 60 יום
    public class DailySalesDataPoint
    {
        public string DateLabel { get; set; } // פורמט: "dd/MM"
        public int KtSales { get; set; }
        public int KesemSales { get; set; }
        public int GoralSales { get; set; }
        public int WebrushSales { get; set; }
    }

    // נתונים חודשיים לגרף 18 חודשים (Stacked/Cumulative)
    public class StackedMonthlySalesVM
    {
        public List<string> MonthLabels { get; set; } = new List<string>(); // פורמט: "MMM yyyy"
        public List<int> KtSales { get; set; } = new List<int>();
        public List<int> KesemSales { get; set; } = new List<int>();
        public List<int> GoralSales { get; set; } = new List<int>();
        public List<int> WebrushSales { get; set; } = new List<int>();
    }
}