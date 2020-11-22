using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class StoresChecked
    {
        [NotMapped]
        public int Value { get; set; }
        [NotMapped]
        public string StoreName { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}
