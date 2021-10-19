using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace KTSite.Models
{
    public class PaymentHistory
    {
        [Key]
        public long Id { get; set; }
        [Required(ErrorMessage = "The Payment Address is required")]
        public int SentFromAddressId { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Range(1, 50000)]
        public double Amount { get; set; }
        [DefaultValue(false)]
        public string Status { get; set; }// status Pending/Approved

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime PayDate { get; set; }
        [MaxLength(100)]
        public string RejectReason { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isChecked { get; set; }
    }
}
