using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PasswordManager.Customizations.Identity
{
    public class CommonPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        private readonly string[] prop_PasswordComuni;
        public CommonPasswordValidator()
        {
            this.prop_PasswordComuni = new[] {
                "password", "abc", "123", "qwerty"
            };
        }

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            IdentityResult var_Risultato;
            if (prop_PasswordComuni.Any(prop_PasswordComuni => password.Contains(prop_PasswordComuni, StringComparison.CurrentCultureIgnoreCase)))
            {
                var_Risultato = IdentityResult.Failed(new IdentityError { Description = "Password troppo comune"} );
            }
            else
            {
                var_Risultato = IdentityResult.Success;
            }
            return Task.FromResult(var_Risultato);
        }
    }
}