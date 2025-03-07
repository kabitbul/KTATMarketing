using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class LitalAmazonOrders
    {
        [Key]
        public int Id { get; set; }
        //[Required(ErrorMessage = "The Payment Address is required")]
        //public int SentFromAddressId { get; set; }
        [Required]
        [MaxLength(20)]
        public string AmazonOrdId { get; set; }
        public string MarketPlace { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Qty { get; set; }   
        public string Asin { get; set; }
        
    }
}
