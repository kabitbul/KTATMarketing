using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class SellersInventoryVM
    {
        public SellersInventory SellersInventory {get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public IEnumerable<SelectListItem> StoresList { get; set; }
        public bool updateAllStores { get; set; }
    }
}
