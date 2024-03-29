﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace KTSite.Models
{
    public class RefundArchive
    {
        [Key]
        public long Id { get; set; }
        public long OriginalId { get; set; }
        [AllowNull]
        public long OrderId { get; set; }
        [AllowNull]
        public long? ReturnId { get; set; }
        [Required]
        // public double Amount { get; set; }
        public int RefundQuantity { get; set; }
        public string RefundedBy { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime RefundDate { get; set; }
        public double AmountRefunded { get; set; }
        public double AmountChargedFromMerch { get; set; }
        public double Cost { get; set; }
        public int Quantity { get; set; }
        public string UserNameId { get; set; }
        [MaxLength(100)]
        public string MerchId { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External
        public int StoreNameId { get; set; }
    }
}
