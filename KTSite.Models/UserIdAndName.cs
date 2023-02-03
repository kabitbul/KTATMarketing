
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    [NotMapped]
    public class UserIdAndName
    {
     
        public string UserNameId { get; set; }
        public string Name { get; set; }
        
    }
}
