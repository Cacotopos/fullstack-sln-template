using Company.Product.Application.Services;
using Company.Product.Contracts.SampleItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Company.Product.Web.Pages.SampleItems;

[Authorize]
public class EditModel(
    SampleItemService service,
    IStringLocalizer<SharedResource> localizer) : PageModel
{
    public IStringLocalizer<SharedResource> Localizer { get; } = localizer;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public sealed class InputModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var item = await service.GetByIdAsync(Id);
        if (item is null) return NotFound();

        Input = new InputModel { Name = item.Name, Description = item.Description };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var request = new UpdateSampleItemRequest(Input.Name, Input.Description);
        await service.UpdateAsync(Id, request);

        return RedirectToPage("/SampleItems/Index");
    }
}
