using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
    }
}
