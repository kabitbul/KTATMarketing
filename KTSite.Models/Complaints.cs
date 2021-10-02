using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    public class Complaints
    {
        [Key]
        public long Id { get; set; }
        public long? OrderId { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string Description { get; set; }
        public string SolutionDesc { get; set; }
        public string HandledBy { get; set; }
        [DefaultValue(false)]
        public bool Solved { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime ModifiedDate { get; set; }
        public string NewTrackingNumber { get; set; }
        [DefaultValue(false)]
        public bool IsAdmin { get; set; }
        [DefaultValue(false)]
        public bool WarehouseResponsibility { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isChecked { get; set; }

        public int StoreId { get; set; }
        public string ProductName { get; set; }
        public string CustName { get; set; }
        public string TicketResolution { get; set; }
        [MaxLength(100)]
        public string MerchId { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External
    }
}
