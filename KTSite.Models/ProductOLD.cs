using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class ProductOLD
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        
        [Required]
        public int InventoryCount { get; set; }
        [Required]
        public int OnTheWayInventory { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
