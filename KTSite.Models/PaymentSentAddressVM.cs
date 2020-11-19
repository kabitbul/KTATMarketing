using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class PaymentSentAddressVM
    {
        public PaymentSentAddress PaymentSentAddress { get; set; }
        public IEnumerable<SelectListItem> paymentType { get; set; }
    }
}
