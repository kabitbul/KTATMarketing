using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class ReturningItemArchive
    {
        [Key]
        public long Id { get; set; }
        public long OriginalId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public string ItemStatus { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string? Comments { get; set; }
        public DateTime DateArrived { get; set; }
        public string CreatedBy { get; set; }

    }
}
