#region

using RegisterMe.Application.CatRegistrations.Dtos;
using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Areas.Visitor.Controllers.ViewModelServices;

public interface IMultipleStepFormService
{
    public Task<MultipleExhibitionVm> InitializeExhibition(TemporaryCatRegistrationDto catRegistration,
        bool editing, bool isConfirmation);

    public CreateCatDayDto HandleExistingCages(CatDayVm day, int registrationToExhibitionId);
}
