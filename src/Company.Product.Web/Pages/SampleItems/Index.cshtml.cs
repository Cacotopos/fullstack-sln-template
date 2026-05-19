using Company.Product.Application.Services;
using Company.Product.Contracts.SampleItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Company.Product.Web.Pages.SampleItems;

[Authorize]
public class IndexModel(
    SampleItemService service,
    IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IStringLocalizer<SharedResource> Localizer { get; } = localizer;
    public IReadOnlyList<SampleItemDto> Items { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        Items = await service.GetAllAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        await service.DeleteAsync(id);
        return RedirectToPage();
    }
}
