#region

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Queries.GetPersonCagesByExhibitionDay;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
public class CagesController(IAuthorizationService authorizationService, IMediator mediator)
    : BaseController(authorizationService, mediator)
{
    public async Task<IActionResult> Index(int exhibitionDayId)
    {
        List<CageDto> personCages =
            await SendQuery(new GetPersonCagesByExhibitionDayQuery { ExhibitionDayId = exhibitionDayId });
        return View(personCages);
    }
}
