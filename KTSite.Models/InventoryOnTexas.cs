using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class InventoryOnTexas
    {
        [Key]
        public int Id { get; set; }
        //[Required(ErrorMessage = "The Payment Address is required")]
        //public int SentFromAddressId { get; set; }
        [Required]
        public string SKU { get; set; }
        public string StorageType { get; set; }//PalletTypeIdentifier
        public int AvailableQty { get; set; }
        public double Weight { get; set; }
        public int PalletId { get; set; }
        
        public DateTime DateCreated { get; set; }
        
    }
}
