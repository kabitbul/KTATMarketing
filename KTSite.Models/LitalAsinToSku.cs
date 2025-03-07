using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class LitalAsinToSku
    {
        [Key]
        public int Id { get; set; }
        //[Required(ErrorMessage = "The Payment Address is required")]
        //public int SentFromAddressId { get; set; }
        [Required]
        [MaxLength(15)]
        public string Asin { get; set; }
        public string ChinaName { get; set; }
        [DefaultValue(true)]
         public bool Restock { get; set; }
         public string ImageUrl { get; set; }
         [DefaultValue(false)]
         public bool RestockNOTDECIDED { get; set; }
        
    }
}
