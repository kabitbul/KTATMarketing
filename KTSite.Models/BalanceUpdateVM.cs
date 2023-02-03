using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KTSite.Models
{
    public class BalanceUpdateVM
    {
        public BalanceUpdatesList balanceUpdate{ get; set; }
        public IEnumerable<SelectListItem> UsersList { get; set; }
        public IEnumerable<SelectListItem> BalanceUpdateActionList { get; set; }
    }
}
