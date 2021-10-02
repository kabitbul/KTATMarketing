using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class OrderArchive
    {
        [Key]
        public long Id { get; set; }
        public long OriginalId { get; set; }
        [Required]
        [MaxLength(15)]
        public string OrderStatus { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(450)]
        public string UserNameId { get; set; }
        public int StoreNameId { get; set; }
        [Required]
        [Range(1,200)]
        public int Quantity { get; set; }
        [Required]
        public string CustName { get; set; }
        [Required]
        public string CustStreet1 { get; set; }
        public string CustStreet2 { get; set; }
        [Required]
        public string CustCity { get; set; }
        [Required]
        public string CustState { get; set; }
        [Required]
        public string CustZipCode { get; set; }
        public string CustPhone { get; set; }

        public double Cost { get; set; }

        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime UsDate { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required]
        //[Index(“IX_Name_DepartmentMaster”, IsClustered = false)]
        public bool IsAdmin { get; set; }
        //[NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string StringDate { get; set; }
        //[NotMapped]
        public string UserNameToShow { get; set; }
        //[NotMapped]
        public string StoreName { get; set; }
        [DefaultValue(false)]
        public bool TrackingUpdated { get; set; }
        [MaxLength(100)]
        public string MerchId { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External
    }
}
