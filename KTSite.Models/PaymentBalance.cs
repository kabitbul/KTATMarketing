using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class PaymentBalance
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }
        public double Balance { get; set; }
        [DefaultValue(false)]
        public bool IsWarehouseBalance { get; set; }
        [DefaultValue(false)]
        public bool AllowNegativeBalance { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isChecked { get; set; }
    }
}
