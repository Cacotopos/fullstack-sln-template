using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Company.Product.Web.Pages;

[Authorize]
public class DashboardModel(IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IStringLocalizer<SharedResource> Localizer { get; } = localizer;

    public void OnGet() { }
}
