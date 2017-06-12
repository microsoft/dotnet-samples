using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer4Authentication.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public virtual int OfficeNumber { get; set; }
    }
}
