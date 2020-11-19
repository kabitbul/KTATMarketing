using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
        [Range(1, 200)]
        public int Quantity { get; set; }
        public string? Comments { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime DateArrived { get; set; }

    }
}
