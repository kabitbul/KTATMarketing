using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KTSite.Models
{
    [NotMapped]
    public class InventoryOnTexasSumList
    {
     
        public string SKU { get; set; }
        public int totalInventory{ get; set; }
        
    }
}
