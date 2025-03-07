using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class LitalAmazonInventory
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(3)]
        public string MarketPlace { get; set; }
        public DateTime UpdateDate { get; set; }
        [Required]
        [MaxLength(20)]
        public string Asin { get; set; }   
        public int AvailableQty { get; set; }
        public int InboundShippedQty { get; set; }
        public int InboundReceivingQty { get; set; }
        public int ReservedQty { get; set; }
        
    }
}
