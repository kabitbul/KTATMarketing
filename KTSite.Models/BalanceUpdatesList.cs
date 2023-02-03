using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    [NotMapped]
    public class BalanceUpdatesList
    {
     
        public string UserNameId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
         public DateTime? PayDate { get; set; }
        public string Action { get; set; }
        public string Comments { get; set; }
        
    }
}
