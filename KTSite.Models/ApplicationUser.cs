using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace KTSite.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get;  set; }
        public string Role { get; set; }
    }
}
