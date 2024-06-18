using Erp___Kurum_Ici_Haberlesme.Models;

namespace Erp___Kurum_Ici_Haberlesme.ViewModel
{
    public class RoleUserViewModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<AppUser>? Users { get; set; }
    }
}
