// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

#region

using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterMe.Application.Exhibitors.Commands.CreateExhibitor;
using RegisterMe.Application.Exhibitors.Commands.UpdateExhibitor;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorByUserId;
using RegisterMe.Application.System.Queries.GetSupportedCountriesQuery;
using RegisterMe.Domain.Entities;

#endregion

namespace WebGui.Areas.Identity.Pages.Account.Manage;

[AllowAnonymous]
public class RegisterExhibitorModel(IMediator mediator, UserManager<ApplicationUser> userManager)
    : PageModel
{
    [BindProperty] public InputModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public bool UserIsRegisteredAsExhibitor { get; set; }

    public async Task OnGetAsync(string returnUrl = null)
    {
        Input = new InputModel();
        ApplicationUser user = await userManager.GetUserAsync(User);
        ExhibitorAndUserDto exhibitor = await mediator.Send(new GetExhibitorByUserIdQuery { UserId = user!.Id });
        ReturnUrl = returnUrl;
        IEnumerable<SelectListItem> countries = (await mediator.Send(new GetSupportedCountriesQuery())).ToList()
            .Select(i => new SelectListItem { Text = i.CountryName, Value = i.CountryCode });
        if (exhibitor != null)
        {
            Input = new InputModel
            {
                Country = exhibitor.Country,
                City = exhibitor.City,
                Street = exhibitor.Street,
                HouseNumber = exhibitor.HouseNumber,
                ZipCode = exhibitor.ZipCode,
                Organization = exhibitor.Organization,
                MemberNumber = exhibitor.MemberNumber,
                IsPartOfCsch = exhibitor.IsPartOfCsch,
                IsPartOfFife = exhibitor.IsPartOfFife,
                EmailToOrganization = exhibitor.EmailToOrganization
            };
        }
        else
        {
            Input = new InputModel
            {
                Country = "CZ",
                City = string.Empty,
                Street = string.Empty,
                HouseNumber = string.Empty,
                ZipCode = string.Empty,
                Organization = string.Empty,
                MemberNumber = string.Empty,
                IsPartOfCsch = true,
                IsPartOfFife = true,
                EmailToOrganization = string.Empty
            };
        }

        Input.Countries = countries;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        ApplicationUser user = await userManager.GetUserAsync(User);
        ExhibitorAndUserDto exhibitor = await mediator.Send(new GetExhibitorByUserIdQuery { UserId = user!.Id });
        returnUrl ??= Url.Content("~/");
        if (!ModelState.IsValid)
        {
            IEnumerable<SelectListItem> countries = (await mediator.Send(new GetSupportedCountriesQuery())).ToList()
                .Select(i => new SelectListItem { Text = i.CountryName, Value = i.CountryCode });
            Input.Countries = countries;
            return Page();
        }

        UpsertExhibitorDto upsertExhibitor = new()
        {
            Country = Input.Country,
            City = Input.City,
            Street = Input.Street,
            HouseNumber = Input.HouseNumber,
            ZipCode = Input.ZipCode,
            Organization = Input.Organization,
            MemberNumber = Input.MemberNumber,
            IsPartOfCsch = Input.IsPartOfCsch,
            EmailToOrganization = Input.EmailToOrganization,
            IsPartOfFife = Input.IsPartOfFife
        };

        if (exhibitor != null)
        {
            await mediator.Send(new UpdateExhibitorCommand { Exhibitor = upsertExhibitor, AspNetUserId = user!.Id });
        }
        else
        {
            await mediator.Send(new CreateExhibitorCommand { Exhibitor = upsertExhibitor, UserId = user!.Id });
        }

        return LocalRedirect(returnUrl);
    }


    public class InputModel
    {
        [Required] [Display(Name = "Country")] public string Country { get; init; }

        [Required] [Display(Name = "City")] public string City { get; init; }

        [Required] [Display(Name = "Street")] public string Street { get; init; }

        [Required]
        [Display(Name = "HouseNumber")]
        public string HouseNumber { get; init; }

        [Required] [Display(Name = "ZipCode")] public string ZipCode { get; init; }

        [Required]
        [Display(Name = "Organization")]
        public string Organization { get; init; }

        [Required]
        [Display(Name = "MemberNumber")]
        public string MemberNumber { get; init; }

        [Required] public bool IsPartOfCsch { get; init; }

        [Required] public bool IsPartOfFife { get; init; }

        [Required] [EmailAddress] public string EmailToOrganization { get; init; }

        [ValidateNever] public IEnumerable<SelectListItem> Countries { get; set; }
    }
}
