using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class ReturnLabelVM
    {
        public ReturnLabel returnLabel {get; set; }
        public IEnumerable<SelectListItem> OrderList { get; set; }
    }
}
