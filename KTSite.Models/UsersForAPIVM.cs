using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KTSite.Models
{
    public class UsersForAPIVM
    {
        public UsersForAPI usersForAPI {get; set; }
        public IEnumerable<SelectListItem> AppUsersList { get; set; }
        //[DefaultValue(false)]
        //public bool AllStores { get; set; }
    }
}
