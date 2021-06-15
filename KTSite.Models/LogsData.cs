using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KTSite.Models
{
    public class LogsData
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(2000)]
        public string Msg1 { get; set; }
        [MaxLength(2000)]
        public string Msg2 { get; set; }
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }
    }
}
