using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    [NotMapped]
    public class TempTableShops
    {
       public string description { get; set; }
       public string stringVal { get; set; }
        public double doubleVal { get; set; }
    }
}
