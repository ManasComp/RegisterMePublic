// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#region

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
    : PageModel
{
    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out");
        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }

        // This needs to be a redirect so that the browser performs a new
        // request and the identity for the user gets updated.
        return RedirectToPage();
    }
}
