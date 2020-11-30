using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace KTSite.Models
{
    public class AdminVATask
    {
        [Key]
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public string UserNameId { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DefaultValue("Now")]
        public DateTime DateCreated { get; set; }
        public string TaskToDo { get; set; }
        [DefaultValue(false)]
        public bool TaskCompleted { get; set; }
        [NotMapped]
        [DefaultValue(false)]
        public bool isChecked { get; set; }
    }
}
