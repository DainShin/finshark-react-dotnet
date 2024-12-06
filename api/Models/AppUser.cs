using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class AppUser : IdentityUser
    {
       public List<Portfolio> portfolios { get; set; } = new List<Portfolio>();
    }
}