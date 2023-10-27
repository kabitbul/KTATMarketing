using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KTSite.Models
{
    public class AsinToSkuVM
    {
        public AsinToSku AsinToSku {get; set; }
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
