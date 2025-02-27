using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace KTSite.Models
{
    public class InventoryOrdersToAmazon
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductAsin { get; set; }
        [Required]
         public string ProductSku { get; set; }
        [Required]
        public int Quantity { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime DateOrdered { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [AllowNull]
        public DateTime DateReceived { get; set; }
        [DefaultValue(false)]
        public bool InboundUpdated { get; set; }
        

    }
}
