using Microsoft.AspNetCore.Identity;

namespace Erp___Kurum_Ici_Haberlesme.Models
{
    public class AppRole : IdentityRole
    {
        public string? RolAd { get; set; }
    }
}
