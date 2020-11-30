using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KTSite.Models
{
    public class AdminVATaskVM
    {
        public AdminVATask AdminVATask {get; set; }
        public IEnumerable<SelectListItem> StoresList { get; set; }
        [DefaultValue(false)]
        public bool AllStores { get; set; }
    }
}
