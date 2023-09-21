using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class Order
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(15)]
        public string OrderStatus { get; set; }
        [Required]
        public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //public Product Product { get; set; }
        [Required]
        [MaxLength(450)]
        public string UserNameId { get; set; }
      //  [ForeignKey("UserNameId")]
        //public ApplicationUser ApplicationUser { get; set; }
        public int StoreNameId { get; set; }
        //[ForeignKey("StoreNameId")]
        //public UserStoreName UserStoreName { get; set; }
        [Required]
        [Range(1,400)]
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
        public double OrdCharge { get; set; }

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
        [MaxLength(20)]// possible values: not handled;copied to warehouse;Tracking uploaded
        public string ToWarehouseStatus { get; set; }// possible values: not handled;copied to warehouse
        public int ExtensiveOrderId { get; set; }
        public string ExtensiveReferenceId { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isChecked { get; set; }
        [NotMapped]
        public string FullAddress { get; set; }
        [NotMapped]
        public bool AllowComplaint { get; set; }
        [NotMapped]
        public bool AllowReturn { get; set; }





    }
}
