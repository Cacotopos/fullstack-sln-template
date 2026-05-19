using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Company.Product.Web.Pages;

public class IndexModel(IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IStringLocalizer<SharedResource> Localizer { get; } = localizer;

    public void OnGet() { }
}
