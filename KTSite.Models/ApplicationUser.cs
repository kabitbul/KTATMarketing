using Microsoft.AspNetCore.Identity;

namespace KTSite.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get;  set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Role { get; set; }
    }
}
