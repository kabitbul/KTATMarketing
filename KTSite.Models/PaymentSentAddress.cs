using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class PaymentSentAddress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "Address Of Payment Type is required")]
        public string PaymentTypeAddress { get; set; }
        [Required(ErrorMessage = "Payment Type is required")]
        public string PaymentType { get; set; }// status Payoneer/Paypal
        public bool IsAdmin { get; set; }// Admin Address
    }
}
