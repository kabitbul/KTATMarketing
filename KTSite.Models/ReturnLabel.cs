using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class ReturnLabel
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public long OrderId { get; set; }
        [Required]
        public string UserNameId { get; set; }
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public string? CommentsToWarehouse { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime DateCreated { get; set; }
        public string ReturnTracking { get; set; }
        [DefaultValue(false)]
        public bool ReturnDelivered { get; set; }
        [Range(1,200)]
        public int ReturnQuantity { get; set; }
        [MaxLength(100)]
        public string MerchId { get; set; }
        [MaxLength(20)]
        public string MerchType { get; set; }//KT/External
    }
}
