using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PasswordManager.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName {get; set;}
        public virtual ICollection<Passwords> Tab_Passwords {get; set;}
    }
}