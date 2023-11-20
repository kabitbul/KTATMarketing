using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class AsinToSku
    {
        [Key]
        public int Id { get; set; }
        //[Required(ErrorMessage = "The Payment Address is required")]
        //public int SentFromAddressId { get; set; }
        [Required]
        [MaxLength(15)]
        public string Asin { get; set; }
        public string Sku { get; set; }
        public string ChinaName { get; set; }
        [DefaultValue(true)]
         public bool RestockUS { get; set; }
         [DefaultValue(true)]
         public bool RestockCA { get; set; }
         // public IEnumerable<SelectListItem> SkuList{ get; set; } 
        
    }
}
