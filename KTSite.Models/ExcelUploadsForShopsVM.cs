using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KTSite.Models
{
    public class ExcelUploadsForShopsVM
    {
        public ExcelUploadsForShops excelUploadsForShops { get; set; }
        public IEnumerable<SelectListItem> StoresList { get; set; }
        public IEnumerable<SelectListItem> StoresListTR { get; set; }
    }
}
