using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KTSite.Models
{
    public class UserGuideline
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Guideline { get; set; }
    }
}
