using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Models
{
    public class PaymentHistoryMerchVM
    {
        public PaymentHistoryMerch PaymentHistoryMerch { get; set; }       
        public string merchName { get; set; }
        public string userNameId { get; set; }
        public string merchType { get; set; }
        public IEnumerable<SelectListItem> PaymentAddress { get; set; }
        public IEnumerable<SelectListItem> PaymentMerch { get; set; }
        public IEnumerable<SelectListItem> PaymentMethod { get; set; }
    }
}
