using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class PaymentMethodMerch
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage = "Address Of Payment Type is required")]
        [MaxLength(100)]
        public string PaymentTypeAddress { get; set; }
        [Required(ErrorMessage = "Payment Type is required")]
        [MaxLength(20)]
        public string PaymentType { get; set; }// status Payoneer/Paypal
        [DefaultValue(false)]
        public bool PrefferdMethod { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External
    }
}
