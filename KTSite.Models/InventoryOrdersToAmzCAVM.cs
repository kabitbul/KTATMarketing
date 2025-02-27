using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class InventoryOrdersToAmzCAVM
    {
        public InventoryOrdersToAmzCA inventoryOrdersToAmzCA { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
