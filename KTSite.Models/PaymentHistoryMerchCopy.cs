using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class PaymentHistoryMerchCopy
    {
        
       
        [MaxLength(100)]
        public string SentFromAddress { get; set; }//the admin address money sent from
        [MaxLength(100)]
        public string SentToAddress { get; set; }//the 
        [MaxLength(10)]
        public string PaymentMethod { get; set; }//paypal/payoneer
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Range(1, 50000)]
        public double Amount { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime PayDate { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External
    }
}
