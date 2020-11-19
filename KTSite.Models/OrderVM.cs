using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class OrderVM
    {
        public Order Orders {get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
        public IEnumerable<SelectListItem> StoresList { get; set; }
        public IEnumerable<SelectListItem> StatesList{ get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string AllOrder { get; set; }
    }
}
