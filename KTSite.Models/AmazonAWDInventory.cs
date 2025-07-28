using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class AmazonAWDInventory
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
        public int totalInboundQuantity { get; set; }
        public int totalOnhandQuantity { get; set; }
        
    }
}
