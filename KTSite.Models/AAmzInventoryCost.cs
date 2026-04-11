using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class AAmzInventoryCost
    {
        [Key]
        public long Id { get; set; }
        public int StoreId { get; set; }
        [Required]
        [MaxLength(3)]
        public string MarketPlace { get; set; }
        public DateTime DateCreated { get; set; }
        public double FBACost{ get; set; }
        public double AWDCost { get; set; }
        public double OnTheWayCost { get; set; }
        
        
    }
}
