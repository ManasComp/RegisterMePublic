// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#region

using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ConfirmEmailModel(UserManager<ApplicationUser> userManager) : PageModel
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    public string ReturnUrl { get; set; }
    public bool WasSuccessful { get; set; }

    public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        if (userId == null || code == null)
        {
            return RedirectToPage("/Index");
        }

        ApplicationUser user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        IdentityResult result = await userManager.ConfirmEmailAsync(user, code);
        StatusMessage = result.Succeeded ? "Děkujeme za potvrzení emailu" : "Chyba během potvrzování emailu";
        WasSuccessful = result.Succeeded;
        return Page();
    }
}
