using System;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class TempSkuAsin
    {
        [Key]
        public int Id { get; set; }
        
        public int storeId { get; set; }
        [MaxLength(2000)]
        public string sku { get; set; }
        [MaxLength(2000)]
        public string asin { get; set; }
    }
}
