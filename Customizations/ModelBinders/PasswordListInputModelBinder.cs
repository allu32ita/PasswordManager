using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using PasswordManager.Models.InputModels;
using PasswordManager.Models.Options;

namespace PasswordManager.Customizations.ModelBinders
{
    public class PasswordListInputModelBinder : IModelBinder
    {
        private readonly IOptionsMonitor<PasswordsOptions> opzioniPasswords;
        public PasswordListInputModelBinder(IOptionsMonitor<PasswordsOptions> opzioniPasswords)
        {
            this.opzioniPasswords = opzioniPasswords;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            string search  = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            string orderby = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            int page       = Convert.ToInt32(bindingContext.ValueProvider.GetValue("Page").FirstValue);
            bool ascending = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("Ascending").FirstValue);

            PasswordsOptions Var_OpzioniPassword = opzioniPasswords.CurrentValue;
            var inputModel = new PasswordListInputModel(search, page, orderby, ascending, (int)Var_OpzioniPassword.PerPage, Var_OpzioniPassword.Order);

            bindingContext.Result = ModelBindingResult.Success(inputModel);
            return Task.CompletedTask;
        }
    }
}