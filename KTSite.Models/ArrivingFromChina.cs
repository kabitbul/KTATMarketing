using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KTSite.Models
{
    public class ArrivingFromChina
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Range(1,1000)]
        public int NumOfBoxes { get; set; }
        [Range(0, 5000)]
        public int Quantity { get; set; }
        
        [DefaultValue("Now")]
        public DateTime DateArriving { get; set; }
        public string Comments { get; set; }
        [DefaultValue(false)]
        public bool UpdatedByAdmin { get; set; }
    }
}
