using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class ReturningItemVM
    {
        public ReturningItem returningItems {get; set; }
        public IEnumerable<SelectListItem> ReturningItemStatusList { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
