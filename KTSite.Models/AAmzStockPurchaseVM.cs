using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class AAmzStockPurchaseVM
    {
        public AAmzStockPurchase aAmzStockPurchase { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
