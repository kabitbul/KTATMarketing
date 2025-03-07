using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class LitalInventoryOrdersToAmazonVM
    {
        public LitalInventoryOrdersToAmazon inventoryOrdersToAmazon { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
