#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Organizations.Commands.ConfirmOrganization;
using RegisterMe.Application.Organizations.Commands.CreateOrganization;
using RegisterMe.Application.Organizations.Commands.DeleteOrganization;
using RegisterMe.Application.Organizations.Commands.UpdateOrganization;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Enums;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Application.Organizations.Queries.GetOrganizations;
using RegisterMe.Domain.Common;
using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
public class OrganizationController(
    IAuthorizationService authorizationService,
    IMediator mediator,
    IConfiguration configuration)
    : BaseController(authorizationService, mediator)
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Detail(int organizationId)
    {
        OrganizationDto organization =
            await SendQuery(new GetOrganizationByIdQuery { OrganizationId = organizationId });
        return View(organization);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(string? searchString,
        OrganizationConfirmationStatus organizationConfirmationStatus = OrganizationConfirmationStatus.Confirmed,
        int pageNumber = 1, int pageSize = 10, HasExhibitions? hasExhibitions = null)
    {
        PaginatedList<OrganizationDto> result =
            await SendQuery(new GetOrganizationsQuery
            {
                HasExhibitions = hasExhibitions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                OrganizationConfirmationStatus = organizationConfirmationStatus,
                SearchString = searchString ?? ""
            });

        OrganizationModel organizationModel = new()
        {
            Organizations = result,
            OrganizationsFilterDto = new OrganizationFilterDto
            {
                SearchString = searchString, OrganizationConfirmationStatus = organizationConfirmationStatus
            }
        };

        return View(organizationModel);
    }

    [HttpGet]
    public Task<IActionResult> CreateOrganization()
    {
        CreateOrganizationDto dto = CreateOrganizationDto.CreateBlankOrganizationDto(GetUserId());
        TempData["secret"] = configuration.GetValue<string?>("GoogleMaps:AutocompleteApiKey");
        return Task.FromResult<IActionResult>(View(dto));
    }


    [HttpPost]
    public async Task<IActionResult> CreateOrganization(CreateOrganizationDto createOrganizationDto)
    {
        if (createOrganizationDto.AdminId != GetUserId())
        {
            ModelState.AddModelError("AdminId", "Vytváření organiace pro Vás není dostupní");
        }

        if (!ModelState.IsValid)
        {
            return View(createOrganizationDto);
        }

        CreateOrganizationCommand command = new() { CreateOrganizationDto = createOrganizationDto };
        Result<int> result = await SendCommand(command, false,
            "Organizace byla úspěšně vytvořena, prosím odhlašte a přihlašte se znovu, abyste ji viděl/a ve svém profilu");
        if (result.IsFailure)
        {
            return View(createOrganizationDto);
        }

        return RedirectToAction("Index", "Administrator");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateOrganization(int organizationId)
    {
        OrganizationDto organization =
            await SendQuery(new GetOrganizationByIdQuery { OrganizationId = organizationId });
        TempData["secret"] = configuration.GetValue<string>("GoogleMaps:AutocompleteApiKey");
        UpdateOrganizationDto updateOrganizationDto = new()
        {
            Id = organization.Id,
            Address = organization.Address,
            Email = organization.Email,
            TelNumber = organization.TelNumber,
            Website = organization.Website,
            Name = organization.Name
        };
        return View(updateOrganizationDto);
    }


    [HttpPost]
    public async Task<IActionResult> UpdateOrganization(UpdateOrganizationDto updateOrganizationDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateOrganizationDto);
        }

        UpdateOrganizationDto command = new()
        {
            Id = updateOrganizationDto.Id,
            Address = updateOrganizationDto.Address,
            Email = updateOrganizationDto.Email,
            TelNumber = updateOrganizationDto.TelNumber,
            Website = updateOrganizationDto.Website,
            Name = updateOrganizationDto.Name
        };

        Result result = await SendCommand(new UpdateOrganizationCommand { OrganizationDto = command }, false,
            "Organizace byla aktualizována");
        if (result.IsFailure)
        {
            return View(updateOrganizationDto);
        }

        return RedirectToAction("Index", "Administrator");
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmOrganization(ConfirmOrganizationCommand confirmOrganizationCommand)
    {
        await SendCommand(confirmOrganizationCommand, true, "Organizace byla potvrzena");
        return RedirectToAction("Detail", new { organizationId = confirmOrganizationCommand.OrganizationId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOrganization(DeleteOrganizationCommand deleteOrganizationCommand)
    {
        await SendCommand(deleteOrganizationCommand, true, "Organizace byla úspěšně smazána");
        return RedirectToAction("Index", "Administrator");
    }
}
