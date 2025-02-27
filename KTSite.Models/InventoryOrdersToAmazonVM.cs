using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class InventoryOrdersToAmazonVM
    {
        public InventoryOrdersToAmazon inventoryOrdersToAmazon { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
