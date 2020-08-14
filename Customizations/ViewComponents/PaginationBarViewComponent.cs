using Microsoft.AspNetCore.Mvc;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Customizations.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IPaginationInfo model)
        {
            return View(model);
        }
    }
}