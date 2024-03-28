using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string City { get; set; }
        public string Picture { get; set; }
        public DateTime? Birthday { get; set; }
        public Gender Gender { get; set; }
    }
}
