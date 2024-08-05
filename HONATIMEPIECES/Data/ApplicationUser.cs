using Microsoft.AspNetCore.Identity;

namespace HONATIMEPIECES.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }

    }
}
