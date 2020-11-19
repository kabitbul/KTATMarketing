using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class PaymentHistoryVM
    {
        public PaymentHistory PaymentHistory { get; set; }
        public IEnumerable<SelectListItem> PaymentAddress { get; set; }
    }
}
