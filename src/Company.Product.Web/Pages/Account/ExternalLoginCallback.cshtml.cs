using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Company.Product.Web.Pages.Account;

public class ExternalLoginCallbackModel(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager) : PageModel
{
    public async Task<IActionResult> OnGetAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/Dashboard");

        if (remoteError is not null)
        {
            ModelState.AddModelError(string.Empty, $"External provider error: {remoteError}");
            return RedirectToPage("/Account/Login");
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return RedirectToPage("/Account/Login");

        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
            return LocalRedirect(returnUrl);

        var email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (email is null)
            return RedirectToPage("/Account/Login");

        var user = new IdentityUser { UserName = email, Email = email };
        var createResult = await userManager.CreateAsync(user);
        if (createResult.Succeeded)
        {
            await userManager.AddLoginAsync(user, info);
            await signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage("/Account/Login");
    }
}
