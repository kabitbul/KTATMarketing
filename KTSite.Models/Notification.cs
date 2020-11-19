using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KTSite.Models
{
    public class Notification
    {
        [Key]
        public long Id { get; set; }
        public string Message { get; set; }
        public string HandledBy { get; set; }
        [DefaultValue(false)]
        public bool Visible { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime DateMsg { get; set; }
    }
}
