using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class PaymentBalanceMerch
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }
        public double Balance { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External

    }
}
