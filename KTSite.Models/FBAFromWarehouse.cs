using System;
using System.ComponentModel.DataAnnotations;


namespace KTSite.Models
{
    public class FBAFromWarehouse
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SKU { get; set; }
        public string ReferenceId { get; set; }
        public int Qty { get; set; }
        public DateTime DateCreated { get; set; }
        
    }
}
