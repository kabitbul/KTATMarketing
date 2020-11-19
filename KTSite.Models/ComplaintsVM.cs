using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KTSite.Models
{
    public class ComplaintsVM
    {
        public Complaints complaints { get; set; }
        public IEnumerable<SelectListItem> OrdersList { get; set; }
        public IEnumerable<SelectListItem> StoresList { get; set; }
        [DefaultValue(false)]
        public bool GeneralNotOrderRelated { get; set; }
    }
}
