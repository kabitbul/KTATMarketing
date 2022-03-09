using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class UserStoreName
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserNameId { get; set; }
        [ForeignKey("UserNameId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string StoreName { get; set; }
        public string UserName { get; set; }

        public bool IsAdminStore { get; set; }
        public long FromOrdId { get; set; }
        public long ToOrdId { get; set; }
    }
}
