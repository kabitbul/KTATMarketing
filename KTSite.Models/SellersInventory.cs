using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class SellersInventory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //public Product Product { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        
        public string UserName { get; set; }
        [Required]
        public int StoreNameId { get; set; }
    }
}
