using Company.Product.Application.Services;
using Company.Product.Contracts.SampleItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Company.Product.Web.Pages.SampleItems;

[Authorize]
public class CreateModel(
    SampleItemService service,
    IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IStringLocalizer<SharedResource> Localizer { get; } = localizer;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public sealed class InputModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var request = new CreateSampleItemRequest(Input.Name, Input.Description);
        await service.CreateAsync(request);

        return RedirectToPage("/SampleItems/Index");
    }
}
