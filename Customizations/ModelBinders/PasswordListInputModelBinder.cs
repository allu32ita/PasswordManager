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
            string search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            int page = Convert.ToInt32(bindingContext.ValueProvider.GetValue("Page").FirstValue);
            string orderby = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            bool ascending = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("Ascending").FirstValue);

            var inputModel = new PasswordListInputModel(search, page, orderby, ascending, opzioniPasswords.CurrentValue);

            bindingContext.Result = ModelBindingResult.Success(inputModel);
            return Task.CompletedTask;
        }
    }
}