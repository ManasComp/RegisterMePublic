// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#region

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Identity.Pages.Account.Manage;

public class PersonalDataModel(
    UserManager<ApplicationUser> userManager)
    : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        return Page();
    }
}
