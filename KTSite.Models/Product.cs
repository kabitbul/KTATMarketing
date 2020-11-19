using KTSite.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductDesc { get; set; }
        public string ProductASIN { get; set; }
        public string ProductURL { get; set; }
        public double Cost { get; set; }
        public double SellersCost { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public int InventoryCount { get; set; }
        [Required]
        public int OnTheWayInventory { get; set; }
        [Required]
        public bool OwnByWarehouse { get; set; }
        [Required]
        public bool ReStock { get; set; }
        [Required]
        public bool AvailableForSellers { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        [Required]
        public bool OOSForSellers { get; set; }
        public string MadeIn { get; set; }
    }
}
