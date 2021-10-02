using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class ArrivingFromChinaVM
    {
        public ArrivingFromChina arrivingFromChina { get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
