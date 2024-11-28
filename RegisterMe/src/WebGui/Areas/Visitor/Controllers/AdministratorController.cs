#region

using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitions;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Domain.Entities;
using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
public class AdministratorController(
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    IAuthorizationService authorizationService) : BaseController(authorizationService, mediator)
{
    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        Guard.Against.Null(user);

        if (user.OrganizationId == null)
        {
            TempData["error"] = "Nejste administrátorem žádné organizace";
            return RedirectToAction("Index", "Home");
        }

        Guard.Against.Null(user.OrganizationId);

        PaginatedList<ExhibitionDto> exhibitions = await SendQuery(new GetExhibitionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = user.OrganizationId,
            OrganizationPublishStatus = OrganizationPublishStatus.All,
            UserId = null,
            SearchString = null,
            ExhibitionStatus = ExhibitionRegistrationStatus.All
        });

        OrganizationDto organization =
            await SendQuery(new GetOrganizationByIdQuery { OrganizationId = user.OrganizationId.Value });

        AdministratorModel exhibitionModel = new() { Exhibitions = exhibitions, OrganizationDto = organization };

        return View(exhibitionModel);
    }
}
