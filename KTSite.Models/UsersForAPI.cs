using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KTSite.Models
{
    public class UsersForAPI
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string UserId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string AuthToken { get; set; }
        [DefaultValue(true)]
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
