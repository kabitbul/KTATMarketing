using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class SellersStatistics
    {
        public string UserNameId { get; set; }
        public string UserName { get; set; }
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
