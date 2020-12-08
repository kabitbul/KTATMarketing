using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class ProductStatistics
    {
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int SevenDays { get; set; }
        public int SixDays { get; set; }
        public int FiveDays { get; set; }
        public int FourDays { get; set; }
        public int ThreeDays { get; set; }
        public int TwoDays { get; set; }
        public int Yesterday { get; set; }
        public int Today { get; set; }
        public double WeeklyAverage { get; set; }
        public string WeeklyAveragestr { get; set; }
        public int TotalSales7 { get; set; }

    }
}
