// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#region

using System.Reflection;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorByUserId;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Identity.Pages.Account.Manage;

public class DownloadPersonalDataModel(
    UserManager<ApplicationUser> userManager,
    ILogger<DownloadPersonalDataModel> logger,
    IMediator mediator)
    : PageModel
{
    public IActionResult OnGet()
    {
        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUser user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        }

        logger.LogInformation("User with ID '{UserId}' asked for their personal data", userManager.GetUserId(User));

        // Only include personal data for download
        ExhibitorAndUserDto exhibitor = await mediator.Send(new GetExhibitorByUserIdQuery { UserId = user.Id });

        Dictionary<string, string> personalData;
        if (exhibitor == null)
        {
            IEnumerable<PropertyInfo> personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            personalData = personalDataProps.ToDictionary(p => p.Name, p => p.GetValue(user)?.ToString() ?? "null");
        }
        else
        {
            IEnumerable<PropertyInfo> exhibitoAdnUserrDataProps = typeof(ExhibitorAndUserDto).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            personalData =
                exhibitoAdnUserrDataProps.ToDictionary(p => p.Name, p => p.GetValue(exhibitor)?.ToString() ?? "null");
        }


        IList<UserLoginInfo> logins = await userManager.GetLoginsAsync(user);
        foreach (UserLoginInfo l in logins)
        {
            personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
        }

        personalData.Add("Authenticator Key", await userManager.GetAuthenticatorKeyAsync(user));

        Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
    }
}
