#region

using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.Cages.Dtos.Combination;
using RegisterMe.Application.Cages.Queries.GetAvailableRentedCageTypes;
using RegisterMe.Application.Cages.Queries.GetPersonCageById;
using RegisterMe.Application.Cages.Queries.RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageId;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Exhibitions.Queries.GetGroupsCanBeRegisteredIn;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Domain.Enums;
using WebGui.Areas.Visitor.Models;

#endregion

namespace WebGui.Areas.Visitor.Controllers.ViewModelServices;

public class MultipleStepFormServices(IMediator mediator)
    : IMultipleStepFormService
{
    private const string YourOwnCagesGroupName = "Vaše vlastní";
    private const string RentedCagesGroupName = "K půjčení";
    private const string AlreadyRentedCagesGroupName = "Už použité";

    public async Task<MultipleExhibitionVm> InitializeExhibition(TemporaryCatRegistrationDto catRegistration,
        bool editing, bool isConfirmation)
    {
        RegistrationToExhibitionDto registrationToExhibition =
            await mediator.Send(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId
            });
        Guard.Against.NotFound(catRegistration.RegistrationToExhibitionId, registrationToExhibition);

        ExhibitorAndUserDto exhibitor = await mediator.Send(new GetExhibitorByIdQuery
        {
            ExhibitorId = registrationToExhibition.ExhibitorId
        });

        BriefExhibitionDto briefExhibition =
            await mediator.Send(new GetExhibitionByIdQuery { ExhibitionId = registrationToExhibition.ExhibitionId });
        List<CatDayVm> catDaysViewModels = await GetCatDayVms(briefExhibition, catRegistration, exhibitor);

        MultipleExhibitionVm multipleExhibitionVm = new()
        {
            RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId,
            DayDetails = catDaysViewModels,
            Disabled = editing,
            RegistrationType = catRegistration.RegistrationType,
            Note = catRegistration.Note,
            Step = (int)StepInRegistration.StartedExhibitionSettings,
            IsConfirmation = isConfirmation
        };

        return multipleExhibitionVm;
    }

    public CreateCatDayDto HandleExistingCages(CatDayVm day, int registrationToExhibitionId)
    {
        SelectListItem? existingCages = day.ExistingCages.Find(x => x.Value == day.SelectedCage);

        (int? cageId, RentedCageGroup? rentedCageId) = existingCages?.Group?.Name switch
        {
            YourOwnCagesGroupName => ((int?)int.Parse(existingCages.Value), (RentedCageGroup?)null),
            RentedCagesGroupName => (null, new RentedCageGroup(existingCages.Value)),
            AlreadyRentedCagesGroupName => (null, new RentedCageGroup(existingCages.Value)),
            _ => throw new ArgumentOutOfRangeException()
        };

        CreateCatDayDto updateCatDayCommand = new()
        {
            ExhibitionDayId = day.ExhibitionDayId,
            ExhibitorsCage = cageId,
            GroupsIds = day.SelectedGroupsPerDay!,
            RentedCageTypeId = rentedCageId,
            Cage = day.SelectDefaultCage
                ? new CreateCageDto { Height = 60, Width = day.Width, Length = day.Length }
                : null
        };

        return updateCatDayCommand;
    }

    private List<SelectListItem> ConvertToSelectLisItem(TemporaryCatDayDto? catDayDto, CagesPerDayDto data)
    {
        SelectListGroup ownCagesGroup = new() { Name = YourOwnCagesGroupName };
        SelectListGroup rentedCagesGroup = new() { Name = RentedCagesGroupName };
        SelectListGroup availableCagesGroup = new() { Name = AlreadyRentedCagesGroupName };
        IEnumerable<SelectListItem> existingCages =
            GetCagesInGroups(data, rentedCagesGroup, ownCagesGroup, availableCagesGroup);

        if (catDayDto?.RentedCageTypeId == null)
        {
            return existingCages.ToList();
        }

        RentingType type = catDayDto.RentedCageTypeId.Unparse().CageType;
        SelectListGroup group = type switch
        {
            RentingType.RentedWithZeroOtherCats => rentedCagesGroup,
            RentingType.RentedWithOneOtherCat or RentingType.RentedWithTwoOtherCats => availableCagesGroup,
            _ => throw new InvalidOperationException()
        };
        SelectListItem myCage = new()
        {
            Value = catDayDto.RentedCageTypeId.ToString(),
            Text = catDayDto.RentedCageTypeId.GetDescription(),
            Group = group
        };

        IEnumerable<SelectListItem> selectListItems = existingCages as SelectListItem[] ?? existingCages.ToArray();
        SelectListItem item = selectListItems.First(x => x.Text == myCage.Text && x.Group.Name == myCage.Group.Name);
        item.Selected = true;

        return selectListItems.ToList();
    }

    private HashSet<SelectListItem> GetCagesInGroups(CagesPerDayDto data, SelectListGroup rentedCagesGroup,
        SelectListGroup ownCagesGroup, SelectListGroup availableCagesGroup)
    {
        HashSet<SelectListItem> existingCages = new(new SelectListItemComparer());

        IEnumerable<SelectListItem> yourCages = data.ExhibitorsCages.Select(x => new SelectListItem
        {
            Value = x.CageId.ToString(), Text = x.Description, Group = ownCagesGroup
        });
        existingCages.UnionWith(yourCages);


        IEnumerable<SelectListItem> cagesForRent = data.AvailableCagesForRent.Select(x => new SelectListItem
        {
            Value = x.RentedTypeId.ToString(), Text = x.RentedTypeId.GetDescription(), Group = rentedCagesGroup
        });
        existingCages.UnionWith(cagesForRent);

        IEnumerable<SelectListItem> alreadyRentedCages = data.AvailableAlreadyRentedCagesByExhibitor.Select(x =>
            new SelectListItem
            {
                Value = x.RentedTypeId.ToString(),
                Text = x.RentedTypeId.GetDescription(),
                Group = availableCagesGroup
            });
        existingCages.UnionWith(alreadyRentedCages);

        return existingCages;
    }

    private async Task<List<CatDayVm>> GetCatDayVms(BriefExhibitionDto briefExhibition,
        TemporaryCatRegistrationDto catRegistration, UpsertExhibitorDto exhibitorDto)
    {
        List<CatDayVm> catDaysViewModels = [];
        List<ExhibitionDayDto> exhibitionDayDtos =
            await mediator.Send(new GetDaysByExhibitionIdQuery { ExhibitionId = briefExhibition.Id });
        IEnumerable<ComboDayDto> days = exhibitionDayDtos
            .Select(x =>
                new ComboDayDto(x, catRegistration.CatDays.Find(y => y.ExhibitionDayId == x.Id)));

        foreach (ComboDayDto day1 in days)
        {
            CagesPerDayDto cagesPerDay = await mediator.Send(new GetAvailableRentedCageTypesQuery
            {
                ExhibitionDayId = day1.ExhibitionDay.Id,
                RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId,
                IsLitter = catRegistration.Litter != null,
                CatRegistrationId = catRegistration.Id
            });

            CageDto cage = await GetCage(catRegistration, day1);
            List<SelectListItem> existingCages = ConvertToSelectLisItem(day1.CatDay, cagesPerDay);
            List<SelectListItem> groupsYouCanRegisterTo =
                (await mediator.Send(new GetGroupsCanBeRegisteredInQuery
                {
                    CatRegistration =
                        new LitterOrExhibitedCatDto
                        {
                            ExhibitedCat = catRegistration.ExhibitedCat,
                            LitterDto = catRegistration.Litter,
                            ExhibitorDto = exhibitorDto
                        },
                    ExhibitionDayId = day1.ExhibitionDay.Id
                }))
                .Select(x => new SelectListItem(x.Name, x.GroupId)).ToList();

            CatDayVm day = new()
            {
                Attendance = day1.CatDay != null,
                Width = cage.Width,
                Length = cage.Length,
                ExistingCages = existingCages,
                ExhibitionDayId = day1.ExhibitionDay.Id,
                Date = day1.ExhibitionDay.Date.ToString("dd.MM."),
                SelectedCage = day1.CatDay?.Cage?.PersonCageId?.ToString()
                               ?? day1.CatDay?.RentedCageTypeId?.ToString()
                               ?? "",
                SelectedGroupsPerDay = day1.CatDay?.GroupsIds ?? [],
                SelectDefaultCage = existingCages.Count != 0 && cage.NumberOfCatsInCage != 1,
                GroupsYouCanRegisterTo = groupsYouCanRegisterTo
            };

            catDaysViewModels.Add(day);
        }

        return catDaysViewModels;
    }

    private async Task<CageDto> GetCage(TemporaryCatRegistrationDto catRegistration, ComboDayDto day1)
    {
        CageDto cage;
        if (day1.CatDay?.Cage?.PersonCageId != null && catRegistration.Id != null)
        {
            cage = await mediator.Send(
                new GetPersonCageByIdQuery { PersonCageId = day1.CatDay.Cage.PersonCageId.Value });
            bool recordAlreadyOwnsCage = catRegistration.Id != null &&
                                         await mediator.Send(
                                             new
                                                 RecordWithGivenExhibitionDayIdAndCatRegistrationIdExistWithCageIdQuery
                                                 {
                                                     CageId = day1.CatDay.Cage.PersonCageId.Value,
                                                     CatRegistrationId = catRegistration.Id.Value,
                                                     ExhibitionDayId = day1.ExhibitionDay.Id
                                                 });
            if (!recordAlreadyOwnsCage)
            {
                cage.NumberOfCatsInCage++;
            }
        }
        else
        {
            bool newCage = day1.CatDay?.Cage?.CreateCage != null;
            cage = new CageDto
            {
                Height = newCage ? day1.CatDay!.Cage!.CreateCage!.Height : 60,
                Width = newCage ? day1.CatDay!.Cage!.CreateCage!.Width : 60,
                Length = newCage ? day1.CatDay!.Cage!.CreateCage!.Length : 60,
                NumberOfCatsInCage = newCage ? 1 : 0
            };
        }

        return cage;
    }

    private record ComboDayDto(ExhibitionDayDto ExhibitionDay, TemporaryCatDayDto? CatDay);
}
