using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class AAmzRefund
    {
        [Key]
        public int Id { get; set; }

        public int StoreId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Marketplace { get; set; }

        [Required]
        [MaxLength(100)]
        public string RefundId { get; set; }

        [Required]
        [MaxLength(30)]
        public string OrderId { get; set; }

        [Required]
        [MaxLength(15)]
        public string Asin { get; set; }

        public DateTime? SaleDate { get; set; }

        public DateTime RefundDate { get; set; }
        public DateTime DateCreated { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetRefundLoss { get; set; }
    }
}