using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class ProductStatistics
    {
        [NotMapped]
        public int ProductId { get; set; }
        [NotMapped]
        public int SevenDays { get; set; }
        [NotMapped]
        public int SixDays { get; set; }
        [NotMapped]
        public int FiveDays { get; set; }
        [NotMapped]
        public int FourDays { get; set; }
        [NotMapped]
        public int ThreeDays { get; set; }
        [NotMapped]
        public int TwoDays { get; set; }
        [NotMapped]
        public int Yesterday { get; set; }
        [NotMapped]
        public int Today { get; set; }
        [NotMapped]
        public double WeeklyAverage { get; set; }

    }
}
