using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class PaymentHistoryArchive
    {
        [Key]
        public long Id { get; set; }
        public long OriginalId { get; set; }
        public int SentFromAddressId { get; set; }
        [Required]
        public string UserNameId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public DateTime PayDate { get; set; }
    }
}
