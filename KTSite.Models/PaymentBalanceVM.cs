using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class PaymentBalanceVM
    {
        public PaymentBalance paymentBalances { get; set; }
        public IEnumerable<SelectListItem> UsersList { get; set; }
    }
}
