using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    [NotMapped]
    public class SkuQtyForAverage
    {
       public string Asin { get; set; } 
       public int Qty { get; set; }
       public DateTime PurchaseDate { get; set; }
        
    }
}
