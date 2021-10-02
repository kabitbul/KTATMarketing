using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class ReturningItem
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ItemStatus { get; set; }
        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
        public string? Comments { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime DateArrived { get; set; }
        public string CreatedBy { get; set; }

    }
}
