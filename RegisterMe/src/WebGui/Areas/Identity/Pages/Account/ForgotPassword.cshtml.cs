// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RegisterMe.Application.Interfaces;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
    : PageModel
{
    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        ApplicationUser user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null || !await userManager.IsEmailConfirmedAsync(user))
            // Don't reveal that the user does not exist or is not confirmed
        {
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        string callbackUrl = Url.Page(
            "/Account/ResetPassword",
            null,
            new { area = "Identity", code },
            Request.Scheme);

        await emailSender.SendEmailAsync(
            Input.Email,
            "Reset hesla",
            $"Své heslo si restartujete <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? throw new InvalidOperationException())}'>kliknutím zde</a>.");

        return RedirectToPage("./ForgotPasswordConfirmation");
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; init; }
    }
}
