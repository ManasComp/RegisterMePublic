#region

using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Application.CatRegistrations.Commands.CreateCatRegistration;
using RegisterMe.Application.CatRegistrations.Commands.DeleteCatRegistration;
using RegisterMe.Application.CatRegistrations.Commands.UpdateCatRegistrationCommand;
using RegisterMe.Application.CatRegistrations.Dtos;
using RegisterMe.Application.CatRegistrations.Enums;
using RegisterMe.Application.CatRegistrations.Queries.GetCatRegistrationById;
using RegisterMe.Application.CatRegistrations.Queries.GetUserCatsNotInExhibition;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorById;
using RegisterMe.Application.Exhibitors.Queries.GetExhibitorByUserId;
using RegisterMe.Application.Organizations.Queries.IsOrganizationAdministrator;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Commands.ChangeAdvertisement;
using RegisterMe.Application.RegistrationToExhibition.Commands.DeleteRegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.RegistrationToExhibition.Queries.ExportRegistrationToExhibitionByExhibitionToZip;
using RegisterMe.Application.RegistrationToExhibition.Queries.ExportRegistrationToExhibitionToZip;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionByExhibitorIdAndExhibitionId;
using RegisterMe.Application.RegistrationToExhibition.Queries.GetRegistrationToExhibitionById;
using RegisterMe.Application.RegistrationToExhibition.Queries.HasActiveRegistrations;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Application.System.Queries.GetSupportedCountriesQuery;
using RegisterMe.Application.System.Queries.RequiresGroup;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;
using RegisterMe.Infrastructure;
using WebGui.Areas.Visitor.Controllers.ViewModelServices;
using WebGui.Areas.Visitor.Models;
using WebGui.Services;

#endregion

// ReSharper disable RedundantAnonymousTypePropertyName

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
[Authorize]
public class RegistrationController(
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    IAuthorizationService authorizationService,
    IMemoryDataService memoryDataService,
    IMultipleStepFormService multipleStepFormServices,
    IWebHostEnvironment webHostEnvironment)
    : BaseController(authorizationService, mediator)
{
    private readonly string _emsValidationUrl = GetWebAddress.WebAddress + "Visitor/Registration/EmsValidator";
    private readonly string _requiresGroupUrl = GetWebAddress.WebAddress + "Visitor/Registration/RequiresGroup";

    [HttpGet]
    [AllowAnonymous]
    public Results<Ok<bool>, BadRequest<string>> EmsValidator([AsParameters] EmsModel model)
    {
        if (string.IsNullOrEmpty(model.Ems))
        {
            return TypedResults.BadRequest("Ems kód je ve špatném formátu");
        }

        Result<EmsCode> emsCodeResult = EmsCode.Create(model.Ems);
        if (emsCodeResult.IsFailure)
        {
            return TypedResults.BadRequest("Ems kód je ve špatném formátu");
        }

        if (!emsCodeResult.Value.CanBeParsed())
        {
            return TypedResults.BadRequest("Ems kód je ve špatném formátu");
        }

        if (!emsCodeResult.Value.VerifyEmsCode(model.Breed, model.Colour))
        {
            return TypedResults.BadRequest("Ems kód neodpovídá zadaným parametrům");
        }

        return TypedResults.Ok(true);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> RegistrationOrLogin(int exhibitionId, string? returnUrl)
    {
        if (User.Identity is { IsAuthenticated: true }) // user is authenticated
        {
            ExhibitorAndUserDto? exhibitor = await SendQuery(new GetExhibitorByUserIdQuery { UserId = GetUserId() });
            if (exhibitor == null) // user has to be exhibitor
            {
                TempData["error"] =
                    "Abyste se mohl/a registrovat na výstavu, musíte být prvně vystavovatelem. Prvně se prosím reigstrujte.";
                string redirectUrl = $"/Identity/Account/Manage/Exhibitor?returnUrl={returnUrl}";
                return LocalRedirect(redirectUrl);
            }

            RegistrationToExhibitionDto? registrationToExhibition = await SendQuery(
                new GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery
                {
                    ExhibitionId = exhibitionId, ExhibitorId = exhibitor.Id
                });

            if (registrationToExhibition is null) // user is not registered to exhibition, so we will register him
            {
                return PersonRegAction(exhibitionId);
            }

            TemporaryCatRegistrationDto? temporaryCatRegistration = await
                memoryDataService.GetTemporaryCatRegistration(registrationToExhibition.Id);

            if (temporaryCatRegistration is not null) // user is registered to exhibition, but he has no active cat registration
            {
                return RedirectToAction(nameof(LitterOrCat),
                    new { registrationToExhibitionId = registrationToExhibition.Id });
            }

            // user is registered to exhibition, but he has no active cat registration
            bool hasPaidRegistrationToExhibition = await SendQuery(
                new HasActiveRegistrationsQuery { ExhibitionId = exhibitionId, UserId = GetUserId(), Paid = true });
            if (hasPaidRegistrationToExhibition)
            {
                throw new ForbiddenAccessException();
            }

            bool hasActiveRegistrations = await SendQuery(new HasActiveRegistrationsQuery
            {
                ExhibitionId = exhibitionId, UserId = GetUserId()
            });
            if (hasActiveRegistrations)
            {
                return RedirectToAction("PayInformation", "Pay",
                    new { registrationToExhibitionId = registrationToExhibition.Id });
            }

            //user has no registered cats
            return RedirectToAction(nameof(LitterOrCat),
                new { registrationToExhibitionId = registrationToExhibition.Id });
        }

        // user is not authenticated, so we will redirect him to login or register page
        BriefExhibitionDto briefExhibition =
            await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = exhibitionId });
        RegisterOrLoginModel vm = new() { Name = briefExhibition.Name, ReturnUrl = returnUrl };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> NavigationInCatRegistrationForm(Target target, int registrationToExhibitionId,
        int step,
        bool disabled)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
        Guard.Against.NotFound(registrationToExhibitionId, catRegistration);

        return catRegistration.RegistrationType switch
        {
            RegistrationType.HomeExhibitedCat when target == Target.Forward => step switch
            {
                (int)StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit => UpsertExhibitionAction(
                    registrationToExhibitionId, disabled),
                (int)StepInRegistration.FinishedExhibitionSettings => SummaryAction(registrationToExhibitionId,
                    disabled),
                _ => throw new ArgumentOutOfRangeException()
            },
            RegistrationType.HomeExhibitedCat => step switch
            {
                (int)StepInRegistration.StartedExhibitionSettings => UpsertCatAction(disabled,
                    catRegistration.RegistrationToExhibitionId, catRegistration.Id),
                (int)StepInRegistration.StartedSummary => UpsertExhibitionAction(registrationToExhibitionId, disabled),
                _ => throw new ArgumentOutOfRangeException()
            },
            RegistrationType.NonHomeExhibitedCat when target == Target.Forward => step switch
            {
                (int)StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit => UpsertParentAction(
                    registrationToExhibitionId, disabled, Gender.Male),
                (int)StepInRegistration.FinishedFather => UpsertParentAction(registrationToExhibitionId, disabled,
                    Gender.Female),
                (int)StepInRegistration.FinishedMother => UpsertExhibitionAction(registrationToExhibitionId, disabled),
                (int)StepInRegistration.FinishedExhibitionSettings => SummaryAction(registrationToExhibitionId,
                    disabled),
                _ => throw new ArgumentOutOfRangeException()
            },
            RegistrationType.NonHomeExhibitedCat => step switch
            {
                (int)StepInRegistration.StartedFather => UpsertCatAction(disabled,
                    catRegistration.RegistrationToExhibitionId, catRegistration.Id),
                (int)StepInRegistration.StartedMother => UpsertParentAction(registrationToExhibitionId, disabled,
                    Gender.Male),
                (int)StepInRegistration.StartedExhibitionSettings => UpsertParentAction(registrationToExhibitionId,
                    disabled, Gender.Female),
                (int)StepInRegistration.StartedSummary => UpsertExhibitionAction(registrationToExhibitionId, disabled),
                _ => throw new ArgumentOutOfRangeException()
            },
            RegistrationType.Litter when target == Target.Forward => step switch
            {
                (int)StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit => UpsertParentAction(
                    registrationToExhibitionId, disabled, Gender.Male),
                (int)StepInRegistration.FinishedFather => UpsertParentAction(registrationToExhibitionId, disabled,
                    Gender.Female),
                (int)StepInRegistration.FinishedMother => UpsertExhibitionAction(registrationToExhibitionId, disabled),
                (int)StepInRegistration.FinishedExhibitionSettings => SummaryAction(registrationToExhibitionId,
                    disabled),
                _ => throw new ArgumentOutOfRangeException()
            },
            RegistrationType.Litter => step switch
            {
                (int)StepInRegistration.StartedFather => UpsertLitterAction(disabled,
                    catRegistration.RegistrationToExhibitionId, registrationToExhibitionId),
                (int)StepInRegistration.StartedMother => UpsertParentAction(registrationToExhibitionId, disabled,
                    Gender.Male),
                (int)StepInRegistration.StartedExhibitionSettings => UpsertParentAction(registrationToExhibitionId,
                    disabled, Gender.Female),
                (int)StepInRegistration.StartedSummary => UpsertExhibitionAction(registrationToExhibitionId, disabled),
                _ => throw new ArgumentOutOfRangeException()
            },
            _ => throw new Exception("Unknown registration type")
        };
    }

    [HttpGet]
    public async Task<IActionResult> OldCatRegistrations(int registrationToExhibitionId, CatRegistrationType type)
    {
        ExhibitorAndUserDto? exhibitor = await SendQuery(new GetExhibitorByUserIdQuery { UserId = GetUserId() });
        Guard.Against.Null(exhibitor);
        List<CatModelP> cats = await SendQuery(new GetUserCatsNotInExhibitionQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId, Type = type
        });


        return View(new PrefillModel { Cats = cats, RegistrationToExhibitionId = registrationToExhibitionId });
    }

    [HttpGet]
    public async Task<IActionResult> PersonRegistration(int exhibitionId,
        bool disabled, int? registrationToExhibitionId)
    {
        if (registrationToExhibitionId != null)
        {
            await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
                Operations.Read);
        }

        bool isPaid = await SendQuery(new HasActiveRegistrationsQuery
        {
            ExhibitionId = exhibitionId, UserId = GetUserId(), Paid = true
        });
        if (isPaid)
        {
            TempData["error"] = "Registrace na tuto výstavu už byla odeslaná";
            return RedirectToAction("Detail", "Exhibition", new { area = "Visitor", id = exhibitionId });
        }

        List<AdvertisementDto> advertisements =
            await SendQuery(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = exhibitionId });
        PersonRegistrationModel model;
        if (registrationToExhibitionId != null) // user is registered to exhibition
        {
            RegistrationToExhibitionDto registrationToExhibition =
                await SendQuery(new GetRegistrationToExhibitionByIdQuery
                {
                    RegistrationToExhibitionId = (int)registrationToExhibitionId
                });

            Guard.Against.NotFound((int)registrationToExhibitionId, registrationToExhibition);

            if (registrationToExhibition.ExhibitionId != exhibitionId) // user is not registered to this exhibition
            {
                throw new ForbiddenAccessException();
            }

            model = PersonRegistrationModel.Initialize(registrationToExhibition, advertisements, exhibitionId,
                registrationToExhibition.ExhibitorId, disabled);
        }
        else
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new NotFoundException("userId", typeof(ApplicationUser).ToString());
            }

            ExhibitorAndUserDto? exhibitor = await SendQuery(new GetExhibitorByUserIdQuery { UserId = user.Id });
            if (exhibitor == null) // user is not exhibitor
            {
                TempData["error"] =
                    "Abyste se mohl/a registrovat na výstavu, musíte být prvně vystavovatelem. Prvně se prosím registrujte.";
                string redirectUrl =
                    $"/Identity/Account/Manage/Exhibitor?returnUrl={Url.Action("PersonRegistration", "Registration", new { area = "Visitor", exhibitionId, disabled, registrationToExhibitionId })}";
                return LocalRedirect(redirectUrl);
            }

            model = PersonRegistrationModel.Initialize(user, exhibitor, advertisements, exhibitionId, disabled);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PersonRegistration(PersonRegistrationModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.RegistrationToExhibitionId != null)
        {
            await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(model.RegistrationToExhibitionId),
                Operations.Read);
        }

        RegistrationToExhibitionDto? registrationToExhibition =
            await SendQuery(
                new GetRegistrationToExhibitionByExhibitorIdAndExhibitionIdQuery
                {
                    ExhibitorId = model.ExhibitorId, ExhibitionId = model.ExhibitionId
                });
        int? registrationToExhibitionId = registrationToExhibition?.Id;
        if (registrationToExhibition != null) // user is already registered to exhibition
        {
            Result result = await SendCommand(
                new ChangeAdvertisementsCommand
                {
                    RegistrationToExhibitionId = registrationToExhibition.Id,
                    AdvertisementId = model.SelectedAdvertisementId
                }, false, null, null, false);
            if (result.IsFailure)
            {
                return View(model);
            }
        }
        else // he is not registered to exhibition, so we will register him
        {
            Result<int> result =
                await SendCommand(
                    model.ConvertToCreateRegistrationToExhibitionCommand(model.SelectedAdvertisementId), true, null,
                    null, false);

            if (result.IsFailure)
            {
                return View(model);
            }

            registrationToExhibitionId = result.IsSuccess ? result.Value : null;
        }

        return RedirectToAction(nameof(LitterOrCat), new { registrationToExhibitionId });
    }

    [HttpGet]
    public async Task<ViewResult> LitterOrCat(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
        bool isTemporary = catRegistration != null;

        LitterOrCatModel model = new()
        {
            IsTemporary = isTemporary, RegistrationToExhibitionId = registrationToExhibitionId
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ContinueInExistingRegistration(int registrationToExhibitionId)
    {
        TemporaryCatRegistrationDto? temporaryCatRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
        Guard.Against.NotFound(registrationToExhibitionId, temporaryCatRegistration);

        return temporaryCatRegistration.RegistrationType switch
        {
            RegistrationType.NonHomeExhibitedCat or RegistrationType.HomeExhibitedCat => UpsertCatAction(false,
                registrationToExhibitionId, null),
            RegistrationType.Litter => UpsertLitterAction(false, registrationToExhibitionId, null),
            _ => throw new Exception()
        };
    }

    [HttpGet]
    public async Task<IActionResult> AllData(int catRegistrationId, IsEditor? editor = null)
    {
        TempData["editor"] = editor;
        List<SelectListItem> countries = (await SendQuery(new GetSupportedCountriesQuery()))
            .Select(x => new SelectListItem(x.CountryName, x.CountryCode)).ToList();
        CatRegistrationDto catRegistration =
            await SendQuery(new GetCatRegistrationByIdQuery { Id = catRegistrationId });

        FatherDto? father = catRegistration.ExhibitedCat?.Father ?? catRegistration.Litter?.Father;
        MotherDto? mother = catRegistration.ExhibitedCat?.Mother ?? catRegistration.Litter?.Mother;

        TemporaryCatRegistrationDto temporary = TemporaryCatRegistrationDto.InitializeFrom(catRegistration);
        RegistrationToExhibitionDto registrationToExhibition = await SendQuery(
            new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId
            });
        BriefExhibitionDto exhibition =
            await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = registrationToExhibition.ExhibitionId });
        bool isOrganizationAdministrator = await SendQuery(new IsOrganizationAdministratorQuery
        {
            UserId = GetUserId(), OrganizationId = exhibition.OrganizationId
        });
        AllVm alVvm = new()
        {
            CatRegistrationId = catRegistrationId,
            RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId,
            LitterModel = catRegistration.Litter != null
                ? LitterModel.InitializeFrom(catRegistration.Litter,
                    catRegistration.RegistrationToExhibitionId,
                    true, StepInRegistration.StartedExhibitedCatAndBreederInit, countries)
                : null,
            ExhibitedCatModel = catRegistration.ExhibitedCat != null
                ? ExhibitedCatModel.InitializeFrom(catRegistration.ExhibitedCat,
                    catRegistration.RegistrationToExhibitionId,
                    true, StepInRegistration.StartedExhibitedCatAndBreederInit, countries)
                : null,
            FatherModel = father != null
                ? CatModel.InitializeFrom(father,
                    catRegistration.RegistrationToExhibitionId, true, catRegistration.RegistrationType,
                    StepInRegistration.StartedFather)
                : null,
            MotherModel = mother != null
                ? CatModel.InitializeFrom(mother,
                    catRegistration.RegistrationToExhibitionId, true, catRegistration.RegistrationType,
                    StepInRegistration.StartedFather)
                : null,
            ExhibitionVm = await multipleStepFormServices.InitializeExhibition(temporary, true, true),
            CanBeEdited = RegistrationToExhibitionService.IsNotPaid(registrationToExhibition.PaymentInfo) ||
                          isOrganizationAdministrator
        };

        return View(alVvm);
    }

    [HttpGet]
    public async Task<IActionResult> PersonReg(int registrationToExhibitionId, bool isAdmin)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });

        List<AdvertisementDto> advertisements = await SendQuery(
            new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = registrationToExhibition.ExhibitionId });

        PersonRegistrationModel model = PersonRegistrationModel.Initialize(registrationToExhibition, advertisements,
            registrationToExhibition.ExhibitionId,
            registrationToExhibition.ExhibitorId, false);

        model.RegistrationToExhibitionId = registrationToExhibition.Id;
        model.SelectedAdvertisementId = registrationToExhibition.AdvertisementId;
        model.Disabled = true;
        TempData["isAdmin"] = isAdmin;

        BriefExhibitionDto exhibition =
            await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = model.ExhibitionId });
        bool isOrganizationAdministrator = await SendQuery(new IsOrganizationAdministratorQuery
        {
            UserId = GetUserId(), OrganizationId = exhibition.OrganizationId
        });

        bool canBeEdited = (RegistrationToExhibitionService.IsNotPaid(registrationToExhibition.PaymentInfo) ||
                            isOrganizationAdministrator) &&
                           advertisements.Count > 1;

        PersonPostEditModel postModel = new() { PersonRegistrationModel = model, CanBeEdited = canBeEdited };


        return View(postModel);
    }

    [HttpGet]
    public async Task<IActionResult> PersonEdit(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });

        List<AdvertisementDto> advertisements =
            await SendQuery(
                new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = registrationToExhibition.ExhibitionId });

        PersonRegistrationModel model = PersonRegistrationModel.Initialize(registrationToExhibition, advertisements,
            registrationToExhibition.ExhibitionId,
            registrationToExhibition.ExhibitorId, false);

        model.RegistrationToExhibitionId = registrationToExhibition.Id;

        model.SelectedAdvertisementId = registrationToExhibition.AdvertisementId;
        model.Disabled = false;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PersonEdit(PersonRegistrationModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(model.RegistrationToExhibitionId),
            Operations.Read);

        Result result = await SendCommand(
            new ChangeAdvertisementsCommand
            {
                RegistrationToExhibitionId = model.RegistrationToExhibitionId!.Value,
                AdvertisementId = model.SelectedAdvertisementId
            }, false, null, null, false);

        if (result.IsFailure)
        {
            return View(model);
        }

        object? isAdmin = TempData["isAdmin"];
        TempData.Remove("isAdmin");

        if (isAdmin is true)
        {
            return RedirectToAction("Detail", "RegistrationToExhibition",
                new { area = Areas.Visitor, registrationToExhibitionId = model.RegistrationToExhibitionId });
        }

        return RedirectToAction("PayInformation", "Pay",
            new { registrationToExhibitionId = model.RegistrationToExhibitionId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRegistrationToRegistration(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);
        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        memoryDataService.DeleteTemporaryCatRegistration(registrationToExhibitionId);

        await SendCommand(
            new DeleteRegistrationToExhibitionCommand { RegistrationToExhibitionId = registrationToExhibitionId });
        return RedirectToAction("Detail", "Exhibition",
            new { area = "Visitor", id = registrationToExhibition.ExhibitionId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRegistrationToRegistrationAdmin(int registrationToExhibitionId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);
        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        memoryDataService.DeleteTemporaryCatRegistration(registrationToExhibitionId);

        await SendCommand(
            new DeleteRegistrationToExhibitionCommand { RegistrationToExhibitionId = registrationToExhibitionId });
        return RedirectToAction("AdminIndex", "RegistrationToExhibition",
            new { area = "Visitor", exhibitionId = registrationToExhibition.ExhibitionId });
    }


    public record PersonPostEditModel
    {
        public required PersonRegistrationModel PersonRegistrationModel { get; init; }
        public required bool CanBeEdited { get; init; }
    }

    #region UpsertCatRegion

    [HttpGet]
    public async Task<ViewResult> UpsertCat(bool disabled, int registrationToExhibitionId, int? catRegistrationId)
    {
        TempData["warning"] = _emsValidationUrl;
        TempData["requiresGroup"] = _requiresGroupUrl;

        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);
        List<SelectListItem> countries = (await SendQuery(new GetSupportedCountriesQuery()))
            .Select(x => new SelectListItem(x.CountryName, x.CountryCode)).ToList();
        ExhibitedCatModel model;
        if (catRegistrationId != null) // user is editing cat registration
        {
            TemporaryCatRegistrationDto? teporaryCatRegistration =
                await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);

            if (teporaryCatRegistration == null)
            {
                CatRegistrationDto catRegistration =
                    await SendQuery(new GetCatRegistrationByIdQuery { Id = catRegistrationId.Value });
                teporaryCatRegistration = TemporaryCatRegistrationDto.InitializeFrom(catRegistration);
            }

            memoryDataService.SetTemporaryCatRegistration(teporaryCatRegistration,
                registrationToExhibitionId);

            model = ExhibitedCatModel.InitializeFrom(teporaryCatRegistration.ExhibitedCat!, registrationToExhibitionId,
                disabled, StepInRegistration.StartedExhibitedCatAndBreederInit, countries);
        }
        else
        {
            TemporaryCatRegistrationDto? catRegistration =
                await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
            if (catRegistration != null) // user is editing cat registration
            {
                model = ExhibitedCatModel.InitializeFrom(catRegistration.ExhibitedCat!, registrationToExhibitionId,
                    disabled, StepInRegistration.StartedExhibitedCatAndBreederInit, countries);
            }

            else
            {
                // user is creating new cat registration
                model = ExhibitedCatModel.InitializeBlank(RegistrationType.NonHomeExhibitedCat,
                    registrationToExhibitionId,
                    disabled,
                    StepInRegistration.StartedExhibitedCatAndBreederInit, countries);
            }
        }


        return View(model);
    }

    [HttpGet]
    public async Task<RedirectToActionResult> DeleteTempAndUpsertCatRegistration(int registrationToExhibitionId)
    {
        ExhibitorAndUserDto? exhibitorId =
            await SendQuery(new GetExhibitorByUserIdQuery { UserId = GetUserId() });
        Guard.Against.Null(exhibitorId);
        List<CatModelP> cats = await SendQuery(new GetUserCatsNotInExhibitionQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId, Type = CatRegistrationType.Cat
        });
        return cats.Count == 0
            ? UpsertCatAction(false, registrationToExhibitionId, null)
            : OldCatRegistrationAction(registrationToExhibitionId, CatRegistrationType.Cat);
    }


    [HttpGet]
    public async Task<RedirectToActionResult> DeleteTempAndUpsertLitter(int registrationToExhibitionId)
    {
        ExhibitorAndUserDto? exhibitorId =
            await SendQuery(new GetExhibitorByUserIdQuery { UserId = GetUserId() });
        Guard.Against.Null(exhibitorId);
        List<CatModelP> cats = await SendQuery(new GetUserCatsNotInExhibitionQuery
        {
            RegistrationToExhibitionId = registrationToExhibitionId, Type = CatRegistrationType.Litter
        });
        return cats.Count == 0
            ? UpsertLitterAction(false, registrationToExhibitionId, null)
            : OldCatRegistrationAction(registrationToExhibitionId, CatRegistrationType.Litter);
    }

    [HttpGet]
    public async Task<RedirectToActionResult> DeleteTemp(int registrationToExhibitionId)
    {
        memoryDataService.DeleteTemporaryCatRegistration(registrationToExhibitionId);
        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });

        return RedirectToAction("Detail", "Exhibition",
            new { area = "Visitor", id = registrationToExhibition.ExhibitionId });
    }


    [HttpGet]
    public async Task<ViewResult> UpsertLitter(bool disabled, int registrationToExhibitionId,
        int? catRegistrationId)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        List<SelectListItem> countries = (await SendQuery(new GetSupportedCountriesQuery()))
            .Select(x => new SelectListItem(x.CountryName, x.CountryCode)).ToList();
        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);

        LitterModel model;
        if (catRegistration != null) // user is editing cat registration
        {
            if (catRegistration.Litter == null)
            {
                model = LitterModel.GetInitializedLitterModel(registrationToExhibitionId, disabled, countries);
            }
            else
            {
                model = LitterModel.InitializeFrom(catRegistration.Litter!, registrationToExhibitionId,
                    disabled, StepInRegistration.StartedExhibitedCatAndBreederInit, countries);
            }
        }
        else if (catRegistrationId != null) // user is editing cat registration
        {
            CatRegistrationDto catRegistrationDto = await SendQuery(new GetCatRegistrationByIdQuery
            {
                Id = catRegistrationId.Value
            });
            if (catRegistrationDto == null)
            {
                throw new NotFoundException("catRegistrationId.ToString()", typeof(CatRegistration).ToString());
            }

            memoryDataService.SetTemporaryCatRegistration(
                TemporaryCatRegistrationDto.InitializeFrom(catRegistrationDto),
                registrationToExhibitionId);

            model = LitterModel.InitializeFrom(catRegistrationDto.Litter!, registrationToExhibitionId,
                disabled, StepInRegistration.StartedExhibitedCatAndBreederInit, countries);
        }
        else
        {
            // user is creating new litter registration
            model = LitterModel.GetInitializedLitterModel(registrationToExhibitionId, disabled, countries);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertLitter(LitterModel model)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(model.RegistrationToExhibitionId),
            Operations.Read);

        RegistrationToExhibitionDto? registrationToExhibition = await
            SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = model.RegistrationToExhibitionId
            });
        DateOnly firstDayOfExhibition = (await SendQuery(new GetDaysByExhibitionIdQuery
        {
            ExhibitionId = registrationToExhibition.ExhibitionId
        })).Select(x => x.Date).Min();

        ServerSideValidate("BreederName", () => VerifyBreederName(model.BreederName, model.IsSameAsExhibitor, false));
        ServerSideValidate("BreederSurname",
            () => VerifyBreederSurname(model.BreederSurname, model.IsSameAsExhibitor, false));
        ServerSideValidate("BreederCountry",
            () => VerifyBreederCountry(model.BreederCountry, model.IsSameAsExhibitor, false));
        ServerSideValidate("DateOfBirth", () => VerifyAge(model.DateOfBirth, firstDayOfExhibition));
        if (!ModelState.IsValid)
        {
            List<SelectListItem> countries = (await SendQuery(new GetSupportedCountriesQuery()))
                .Select(x => new SelectListItem(x.CountryName, x.CountryCode)).ToList();
            model.Countries = countries;
            return View(model);
        }


        if (model.IsSameAsExhibitor) // we initialize breeder from exhibitor
        {
            if (registrationToExhibition == null)
            {
                throw new NotFoundException("registrationToExhibitionId.ToString()",
                    typeof(RegistrationToExhibition).ToString());
            }

            ExhibitorAndUserDto exhibitor = await SendQuery(new GetExhibitorByIdQuery
            {
                ExhibitorId = registrationToExhibition.ExhibitorId
            });
            model.BreederName = exhibitor.FirstName;
            model.BreederSurname = exhibitor.LastName;
            model.BreederCountry = exhibitor.Country;
        }

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(model.RegistrationToExhibitionId);

        if (catRegistration == null)
        {
            // create
            BriefExhibitionDto briefExhibition =
                await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = registrationToExhibition.ExhibitionId });
            if (briefExhibition == null)
            {
                throw new NotFoundException("exhibitionId.ToString()", typeof(Exhibition).ToString());
            }

            catRegistration = model.GetAsCatRegistrationTemporary();
        }
        else
        {
            //edit
            catRegistration.Litter!.NameOfBreedingStation = model.NameOfBreedingStation;
            catRegistration.Litter!.BirthDate = model.DateOfBirth;
            catRegistration.Litter.Breed = model.Breed;
            catRegistration.Litter.Breeder.FirstName = model.BreederName!;
            catRegistration.Litter.Breeder.LastName = model.BreederSurname!;
            catRegistration.Litter.Breeder.Country = model.BreederCountry!;
            catRegistration.Litter.Breeder.BreederIsSameAsExhibitor = model.IsSameAsExhibitor;
            catRegistration.Litter.PassOfOrigin = model.PassOfOrigin;
            catRegistration.StepInRegistration = StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit;
        }

        memoryDataService.SetTemporaryCatRegistration(catRegistration,
            model.RegistrationToExhibitionId);

        return await NavigationInCatRegistrationForm(Target.Forward, model.RegistrationToExhibitionId,
            (int)StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit, model.Disabled);
    }

    [HttpGet]
    public async Task<IActionResult> PrefillData(int prefillFromCatRegistrationId, int registrationToExhibitionId)
    {
        CatRegistrationDto cat =
            await SendQuery(new GetCatRegistrationByIdQuery { Id = prefillFromCatRegistrationId });
        TemporaryCatRegistrationDto temporaryCatRegistration = TemporaryCatRegistrationDto.InitializeFrom(cat);
        TemporaryCatRegistrationDto newTemporaryCatRegistration = new()
        {
            ExhibitedCat = temporaryCatRegistration.ExhibitedCat,
            Litter = temporaryCatRegistration.Litter,
            RegistrationToExhibitionId = registrationToExhibitionId,
            Note = temporaryCatRegistration.Note,
            StepInRegistration = temporaryCatRegistration.StepInRegistration,
            CatDays = [],
            Id = null
        };

        memoryDataService.SetTemporaryCatRegistration(newTemporaryCatRegistration, registrationToExhibitionId);

        TempData["warn"] = "Zkontrulujte všechny údaje";
        return RedirectToAction(
            cat.RegistrationType is RegistrationType.NonHomeExhibitedCat or RegistrationType.HomeExhibitedCat
                ? nameof(UpsertCat)
                : nameof(UpsertLitter),
            new { registrationToExhibitionId = registrationToExhibitionId });
    }

    [HttpPost]
    public async Task<IActionResult> UpsertCat(ExhibitedCatModel model)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(model.RegistrationToExhibitionId),
            Operations.Read);

        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = model.RegistrationToExhibitionId
            });
        DateOnly firstDayOfExhibition = (await SendQuery(new GetDaysByExhibitionIdQuery
        {
            ExhibitionId = registrationToExhibition.ExhibitionId
        })).Select(x => x.Date).Min();

        ServerSideValidate("BreederName",
            () => VerifyBreederName(model.BreederName, model.IsSameAsExhibitor, model.HasBreeder));
        ServerSideValidate("BreederSurname",
            () => VerifyBreederSurname(model.BreederSurname, model.IsSameAsExhibitor, model.HasBreeder));
        ServerSideValidate("BreederCountry",
            () => VerifyBreederCountry(model.BreederCountry, model.IsSameAsExhibitor, model.HasBreeder));
        ServerSideValidate("Group", () => VerifyGroup(model.Group, model.Ems));
        ServerSideValidate("BreedingBook",
            () => VerifyBreedingBook(model.BreedingBook, model.DateOfBirth, model.IsHomeCat));
        ServerSideValidate("Ems", () => VerifyEms(model.Ems));
        ServerSideValidate("Colour", () => VerifyColour(model.Colour, model.Ems));
        ServerSideValidate("Colour", () => VerifyColour(model.Colour, model.IsHomeCat));
        ServerSideValidate("DateOfBirth", () => VerifyCatAge(model.DateOfBirth, firstDayOfExhibition));
        if (!ModelState.IsValid)
        {
            TempData["warning"] = _emsValidationUrl;
            TempData["requiresGroup"] = _requiresGroupUrl;
            List<SelectListItem> countries = (await SendQuery(new GetSupportedCountriesQuery()))
                .Select(x => new SelectListItem(x.CountryName, x.CountryCode)).ToList();
            model.Countries = countries;
            return View(model);
        }

        Guard.Against.NotFound(model.RegistrationToExhibitionId, registrationToExhibition);
        if (model.IsSameAsExhibitor) // we initialize breeder from exhibitor
        {
            ExhibitorAndUserDto exhibitor =
                await SendQuery(new GetExhibitorByIdQuery { ExhibitorId = registrationToExhibition.ExhibitorId });
            model.BreederName = exhibitor.FirstName;
            model.BreederSurname = exhibitor.LastName;
            model.BreederCountry = exhibitor.Country;
        }

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(model.RegistrationToExhibitionId);

        if (catRegistration == null)
        {
            // create
            BriefExhibitionDto briefExhibition =
                await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = registrationToExhibition.ExhibitionId });
            Guard.Against.NotFound(registrationToExhibition.ExhibitionId, briefExhibition);
            catRegistration = model.GetAsCatRegistrationTemporary();
        }
        else
        {
            //edit
            if (model.HasBreeder)
            {
                catRegistration.ExhibitedCat!.Breeder = new BreederDto
                {
                    FirstName = model.BreederName ?? string.Empty,
                    LastName = model.BreederSurname ?? string.Empty,
                    BreederIsSameAsExhibitor = model.IsSameAsExhibitor,
                    Country = model.BreederCountry ?? string.Empty
                };
            }
            else
            {
                catRegistration.ExhibitedCat!.Breeder = null;
            }

            catRegistration.ExhibitedCat.BirthDate = model.DateOfBirth;
            catRegistration.ExhibitedCat.Breed = model.Breed;
            catRegistration.ExhibitedCat.Colour = model.Colour;
            catRegistration.ExhibitedCat.Neutered = model.Castrated;
            catRegistration.ExhibitedCat.TitleBeforeName = model.TitleBeforeName;
            catRegistration.ExhibitedCat.TitleAfterName = model.TitleAfterName;
            catRegistration.ExhibitedCat.Name = model.Name;
            catRegistration.ExhibitedCat.Ems = model.Ems;
            catRegistration.ExhibitedCat.PedigreeNumber = model.BreedingBook;
            catRegistration.ExhibitedCat.Sex = model.Gender;
            catRegistration.StepInRegistration = StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit;
            catRegistration.ExhibitedCat.IsHomeCat = model.IsHomeCat;
            catRegistration.ExhibitedCat.Group = model.Group;
        }


        if (model.RegistrationType == RegistrationType.HomeExhibitedCat)
        {
            catRegistration.ExhibitedCat!.Father = null;
            catRegistration.ExhibitedCat!.Mother = null;
        }


        memoryDataService.SetTemporaryCatRegistration(catRegistration,
            model.RegistrationToExhibitionId);

        return await NavigationInCatRegistrationForm(Target.Forward, model.RegistrationToExhibitionId,
            (int)StepInRegistration.FinishedExhibitedCatAndLitterAndBreederInit, model.Disabled);
    }


    [HttpGet]
    public async Task<IActionResult> UpsertParent(int registrationToExhibitionId, bool disabled, Gender gender)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        TempData["warning"] = _emsValidationUrl;

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
        Guard.Against.NotFound(registrationToExhibitionId, catRegistration);

        CatModel catModel;
        if ((catRegistration.ExhibitedCat != null && (gender == Gender.Female
                ? catRegistration.ExhibitedCat.Mother != null
                : catRegistration.ExhibitedCat.Father != null))
            || catRegistration.Litter != null)
        {
            ParentDto parent = gender == Gender.Male
                ? catRegistration.Litter?.Father ?? catRegistration.ExhibitedCat?.Father ?? new FatherDto()
                : catRegistration.Litter?.Mother ?? catRegistration.ExhibitedCat?.Mother ?? new MotherDto();
            catModel = CatModel.InitializeFrom(
                parent,
                catRegistration.RegistrationToExhibitionId, disabled,
                catRegistration.RegistrationType,
                gender == Gender.Female ? StepInRegistration.StartedMother : StepInRegistration.StartedFather);
        }
        else
        {
            catModel = new CatModel
            {
                RegistrationType = catRegistration.RegistrationType,
                Gender = gender,
                Step = gender == Gender.Male
                    ? (int)StepInRegistration.StartedFather
                    : (int)StepInRegistration.StartedMother,
                Disabled = disabled,
                RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId
            };
        }

        return View("CatRegistration", catModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertParent(CatModel vm)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(vm.RegistrationToExhibitionId),
            Operations.Read);

        ServerSideValidate("Ems", () => VerifyEms(vm.Ems));
        ServerSideValidate("Colour", () => VerifyColour(vm.Colour, vm.Ems));
        if (!ModelState.IsValid)
        {
            TempData["warning"] = _emsValidationUrl;
            return View("CatRegistration", vm);
        }

        RegistrationToExhibitionDto registrationToExhibition = await SendQuery(
            new GetRegistrationToExhibitionByIdQuery { RegistrationToExhibitionId = vm.RegistrationToExhibitionId });
        Guard.Against.Null(registrationToExhibition);

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(vm.RegistrationToExhibitionId);
        Guard.Against.Null(catRegistration);

        switch (catRegistration.RegistrationType)
        {
            case RegistrationType.NonHomeExhibitedCat when vm.Gender == Gender.Female:
                catRegistration.ExhibitedCat!.Mother = CatModel.ConvertToMother(vm);
                break;
            case RegistrationType.NonHomeExhibitedCat:
                catRegistration.ExhibitedCat!.Father = CatModel.ConvertToFather(vm);
                break;
            case RegistrationType.Litter when vm.Gender == Gender.Female:
                catRegistration.Litter!.Mother = CatModel.ConvertToMother(vm);
                break;
            case RegistrationType.Litter:
                catRegistration.Litter!.Father = CatModel.ConvertToFather(vm);
                break;
            case RegistrationType.HomeExhibitedCat:
            default:
                throw new ArgumentOutOfRangeException();
        }

        catRegistration.StepInRegistration = vm.Gender == Gender.Female
            ? StepInRegistration.FinishedMother
            : StepInRegistration.FinishedFather;

        return vm.Gender == Gender.Male
            ? UpsertParentAction(vm.RegistrationToExhibitionId, vm.Disabled, Gender.Female)
            : UpsertExhibitionAction(vm.RegistrationToExhibitionId, vm.Disabled);
    }

    [HttpGet]
    public async Task<IActionResult> UpsertExhibition(int registrationToExhibitionId, bool disabled)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
        Guard.Against.Null(catRegistration);
        MultipleExhibitionVm vm =
            await multipleStepFormServices.InitializeExhibition(catRegistration, disabled, false);
        return View("Exhibition", vm);
    }

    [HttpPost]
    public async Task<IActionResult> UpsertExhibition(MultipleExhibitionVm vm)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(vm.RegistrationToExhibitionId),
            Operations.Read);

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(vm.RegistrationToExhibitionId);
        Guard.Against.Null(catRegistration);

        MultipleExhibitionVm initializedVm =
            await multipleStepFormServices.InitializeExhibition(catRegistration, vm.Disabled, false);

        for (int i = 0; i < initializedVm.DayDetails.Count; i++)
        {
            vm.DayDetails[i].Date = initializedVm.DayDetails[i].Date;
            ModelState.MarkFieldValid("DayDetails[" + i + "].Date");

            vm.DayDetails[i].ExistingCages = initializedVm.DayDetails[i].ExistingCages;
            vm.DayDetails[i].GroupsYouCanRegisterTo = initializedVm.DayDetails[i].GroupsYouCanRegisterTo;
            ModelState.MarkFieldValid("DayDetails[" + i + "].GroupsYouCanRegisterTo");
        }

        foreach (CatDayVm day in vm.DayDetails.Where(x => x.Attendance))
        {
            ServerSideValidate("Width", () => VerifyCageParametr(day.Width, day.SelectDefaultCage));
            ServerSideValidate("Length", () => VerifyCageParametr(day.Length, day.SelectDefaultCage));
        }

        if (!vm.DayDetails.Exists(x => x.Attendance))
        {
            ModelState.AddModelError("DayDetails", "Musíte vybrat alespoň jeden den");
        }

        foreach (ValidationResult? validationResult in from day in vm.DayDetails
                 let validationContext = new ValidationContext(day)
                 let validationResults = (List<ValidationResult>) []
                 where !Validator.TryValidateObject(day, validationContext, validationResults, true)
                 from validationResult in validationResults
                 select validationResult)
        {
            ModelState.AddModelError(validationResult.MemberNames.First(),
                validationResult.ErrorMessage ?? "Error");
        }

        if (catRegistration.Litter != null)
        {
            foreach (CatDayVm dat in vm.DayDetails.Where(x => x.Attendance))
            {
                if (dat is { SelectDefaultCage: false, Length: < 100 })
                {
                    ModelState.AddModelError(CommandValidation, "Délka klece musí být alespoň 100 cm");
                }
            }
        }
        else
        {
            foreach (CatDayVm dat in vm.DayDetails.Where(x => x.Attendance))
            {
                if (dat is { SelectDefaultCage: false, Length: < 50 })
                {
                    ModelState.AddModelError(CommandValidation,
                        "Délka klece musí být alespoň 50 cm");
                }
            }
        }

        foreach (CatDayVm dat in vm.DayDetails.Where(x => x.Attendance))
        {
            if (dat is not { SelectDefaultCage: false })
            {
                continue;
            }

            if (dat.Width < 50)
            {
                ModelState.AddModelError(CommandValidation,
                    "Šířka klece musí být alespoň 50 cm");
            }

            if (dat.Length < 50)
            {
                ModelState.AddModelError(CommandValidation,
                    "Délka klece musí být alespoň 50 cm");
            }

            if (dat.Width > 500)
            {
                ModelState.AddModelError(CommandValidation,
                    "Šířka klece nesmí být větší než 500 cm");
            }

            if (dat.Length > 500)
            {
                ModelState.AddModelError(CommandValidation,
                    "Délka klece nesmí být větší než 500 cm");
            }
        }

        if (!ModelState.IsValid)
        {
            return View("Exhibition", vm);
        }


        List<TemporaryCatDayDto> updateCatDayCommands = [];
        foreach (CatDayVm? day in vm.DayDetails.Where(day => day.Attendance))
        {
            if (!day.SelectDefaultCage) // we filled in the new cage data
            {
                //create cage
                TemporaryCatDayDto updateCatDayCommand = new()
                {
                    ExhibitionDayId = day.ExhibitionDayId,
                    Cage = new PersonCageDto
                    {
                        CreateCage = new CreateCageDto { Width = day.Width, Length = day.Length, Height = 60 }
                    },
                    GroupsIds = day.SelectedGroupsPerDay!,
                    RentedCageTypeId = null
                };
                updateCatDayCommands.Add(updateCatDayCommand);
            }

            else // we use existing cages
            {
                CreateCatDayDto updateCatDayCommand =
                    multipleStepFormServices.HandleExistingCages(day, catRegistration.RegistrationToExhibitionId);
                TemporaryCatDayDto updateCatDayCommandT = new()
                {
                    ExhibitionDayId = updateCatDayCommand.ExhibitionDayId,
                    Cage = new PersonCageDto { PersonCageId = updateCatDayCommand.ExhibitorsCage },
                    GroupsIds = updateCatDayCommand.GroupsIds,
                    RentedCageTypeId = updateCatDayCommand.RentedCageTypeId
                };
                updateCatDayCommands.Add(updateCatDayCommandT);
            }
        }

        catRegistration.Note = vm.Note;
        catRegistration.CatDays = updateCatDayCommands;
        catRegistration.StepInRegistration = StepInRegistration.FinishedExhibitionSettings;
        memoryDataService.SetTemporaryCatRegistration(catRegistration, vm.RegistrationToExhibitionId);

        return SummaryAction(vm.RegistrationToExhibitionId, vm.Disabled);
    }

    [HttpGet]
    public async Task<IActionResult> Summary(int registrationToExhibitionId, bool disabled)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(registrationToExhibitionId),
            Operations.Read);

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(registrationToExhibitionId);
        if (catRegistration == null)
        {
            throw new NotFoundException("catRegistrationId.ToString()", typeof(CatRegistration).ToString());
        }

        RegistrationToExhibitionDto personRegistration =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        SummaryVm summaryVm = new()
        {
            CatRegistrationDto = catRegistration,
            PersonRegistrationDto = personRegistration.PersonRegistration,
            Step = (int)StepInRegistration.StartedSummary,
            Disabled = disabled,
            RegistrationToExhibitionId = registrationToExhibitionId,
            RegistrationType = catRegistration.RegistrationType
        };

        return View("Summary", summaryVm);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadRegistrationAsZip(int registrationToExhibitionId)
    {
        Stream memoryStream = await SendQuery(new ExportRegistrationToExhibitionToZipQuery
            {
                Id = registrationToExhibitionId,
                WebUrl = GetWebAddress.WebAddress,
                RootPath = webHostEnvironment.WebRootPath
            }
        );
        RegistrationToExhibitionDto registrationToExhibition =
            await SendQuery(new GetRegistrationToExhibitionByIdQuery
            {
                RegistrationToExhibitionId = registrationToExhibitionId
            });
        ExhibitorAndUserDto exhibitor = await SendQuery(new GetExhibitorByIdQuery
        {
            ExhibitorId = registrationToExhibition.ExhibitorId
        });

        string exhibitorName = exhibitor.FirstName + "-" + exhibitor.LastName + "-" + exhibitor.MemberNumber;
        string date = DateTime.Now.ToString("dd-MM_HH-mm");
        string fileName = exhibitorName + "_" + date + ".zip";

        return File(memoryStream, "application/zip", fileName);
    }


    [HttpGet]
    public async Task<IActionResult> DownloadExhibitionDataAsZip(int exhibitionId)
    {
        Stream memoryStream = await SendQuery(new ExportRegistrationToExhibitionByExhibitionToZipQuery
            {
                Id = exhibitionId, WebUrl = GetWebAddress.WebAddress, RootPath = webHostEnvironment.WebRootPath
            }
        );

        BriefExhibitionDto exhibition = await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = exhibitionId });
        string date = DateTime.Now.ToString("dd-MM_HH-mm");
        string fileName = date + "_" + exhibition.Name + ".zip";

        return File(memoryStream, "application/zip", fileName);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCatRegistration(int catRegistrationId)
    {
        int registrationToExhibitionId =
            (await SendQuery(new GetCatRegistrationByIdQuery { Id = catRegistrationId }))
            .RegistrationToExhibitionId;
        await SendCommand(new DeleteCatRegistrationCommand { CatRegistrationId = catRegistrationId });
        return RedirectToAction("PayInformation", "Pay",
            new { registrationToExhibitionId });
    }

    [HttpPost]
    public async Task<IActionResult> Summary(SummaryVm vm)
    {
        await AuthorizeAsync(User, new AuthorizeRegistrationToExhibitionId(vm.RegistrationToExhibitionId),
            Operations.Read);

        ModelState.MarkFieldValid("PersonRegistrationDto");
        if (!ModelState.IsValid)
        {
            return View("Summary", vm);
        }

        TemporaryCatRegistrationDto? catRegistration =
            await memoryDataService.GetTemporaryCatRegistration(vm.RegistrationToExhibitionId);
        if (catRegistration == null)
        {
            throw new NotFoundException("catRegistrationId", typeof(CatRegistration).ToString());
        }

        catRegistration.StepInRegistration = StepInRegistration.FinishedSummary;

        List<CreateCatDayDto> catDays = [];
        catDays.AddRange(catRegistration.CatDays.Select(day => new CreateCatDayDto
        {
            ExhibitionDayId = day.ExhibitionDayId,
            GroupsIds = day.GroupsIds,
            RentedCageTypeId = day.RentedCageTypeId,
            ExhibitorsCage = day.Cage?.PersonCageId,
            Cage = day.Cage?.CreateCage
        }));

        Result result;
        if (catRegistration.Id == null)
        {
            CreateCatRegistrationCommand createCatRegistrationCommand = new()
            {
                CatRegistration = new CreateCatRegistrationDto
                {
                    Litter = catRegistration.Litter,
                    CatDays = catDays,
                    ExhibitedCat = catRegistration.ExhibitedCat,
                    Note = catRegistration.Note,
                    RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId
                }
            };
            result = await SendCommand(createCatRegistrationCommand, true, null, null, false);
        }
        else
        {
            UpdateCatRegistrationCommand updateCatRegistrationCommand = new()
            {
                CatRegistration = new UpdateCatRegistrationDto
                {
                    Litter = catRegistration.Litter,
                    Id = (int)catRegistration.Id,
                    CatDays = catDays,
                    ExhibitedCat = catRegistration.ExhibitedCat,
                    Note = catRegistration.Note
                }
            };
            result = await SendCommand(updateCatRegistrationCommand, true, null, null, false);
        }

        if (result.IsFailure)
        {
            RegistrationToExhibitionDto personRegistration =
                await SendQuery(new GetRegistrationToExhibitionByIdQuery
                {
                    RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId
                });
            SummaryVm summaryVm = new()
            {
                CatRegistrationDto = catRegistration,
                PersonRegistrationDto = personRegistration.PersonRegistration,
                Step = (int)StepInRegistration.StartedSummary,
                Disabled = false,
                RegistrationToExhibitionId = catRegistration.RegistrationToExhibitionId,
                RegistrationType = catRegistration.RegistrationType
            };

            return View("Summary", summaryVm);
        }

        memoryDataService.DeleteTemporaryCatRegistration(vm.RegistrationToExhibitionId);

        object? data = TempData["editor"];
        TempData.Remove("editor");
        return data switch
        {
            (int)IsEditor.Admin => RedirectToAction("Detail", "RegistrationToExhibition",
                new { area = Areas.Visitor, registrationToExhibitionId = vm.RegistrationToExhibitionId }),
            (int)IsEditor.User => RedirectToAction("PayInformation", "Pay",
                new { registrationToExhibitionId = vm.RegistrationToExhibitionId }),
            _ => View("NextOrPay", catRegistration.RegistrationToExhibitionId)
        };
    }

    #endregion

    #region Redirects

    private RedirectToActionResult UpsertCatAction(bool disabled, int registrationToExhibitionId,
        int? catRegistrationId)
    {
        return RedirectToAction(nameof(UpsertCat),
            new { disabled = disabled, registrationToExhibitionId, catRegistrationId });
    }

    private RedirectToActionResult PersonRegAction(int exhibitionId, int? registrationToExhibitionId = null)
    {
        return RedirectToAction(nameof(PersonRegistration),
            new { exhibitionId = exhibitionId, registrationToExhibitionID = registrationToExhibitionId });
    }

    private RedirectToActionResult UpsertParentAction(int catRegistrationId, bool disabled, Gender gender)
    {
        return RedirectToAction(nameof(UpsertParent),
            new { registrationToExhibitionId = catRegistrationId, disabled = disabled, gender = gender });
    }

    private RedirectToActionResult UpsertLitterAction(bool disabled, int registrationToExhibitionId,
        int? catRegistrationId)
    {
        return RedirectToAction(nameof(UpsertLitter),
            new { disabled, registrationToExhibitionId, catRegistrationId });
    }

    private RedirectToActionResult UpsertExhibitionAction(int registrationToExhibitionId, bool? disabled)
    {
        return RedirectToAction(nameof(UpsertExhibition),
            new { registrationToExhibitionId = registrationToExhibitionId, disabled = disabled });
    }

    private RedirectToActionResult OldCatRegistrationAction(int registrationToExhibitionId,
        CatRegistrationType catRegistrationType)
    {
        return RedirectToAction(nameof(OldCatRegistrations),
            new { registrationToExhibitionId = registrationToExhibitionId, type = catRegistrationType });
    }

    private RedirectToActionResult SummaryAction(int registrationToExhibitionId, bool disabled)
    {
        return RedirectToAction(nameof(Summary),
            new { registrationToExhibitionId = registrationToExhibitionId, disabled = disabled });
    }

    #endregion Redirects

    #region Validation

    private JsonResult VerifyBreeder(string? text, bool isSameAsExhibitor, bool hasBreeder)
    {
        if (isSameAsExhibitor || !string.IsNullOrEmpty(text) || !hasBreeder)
        {
            return Json(true);
        }

        return Json("Toto je povinné pole");
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyBreederSurname(string? breederSurname, bool isSameAsExhibitor, bool hasBreeder)
    {
        return VerifyBreeder(breederSurname, isSameAsExhibitor, hasBreeder);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyBreederCountry(string? breederSurname, bool isSameAsExhibitor, bool hasBreeder)
    {
        return VerifyBreeder(breederSurname, isSameAsExhibitor, hasBreeder);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyBreederName(string? breederName, bool isSameAsExhibitor, bool hasBreeder)
    {
        return VerifyBreeder(breederName, isSameAsExhibitor, hasBreeder);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyGroup(int? group, string ems)
    {
        bool isRequired = EmsCode.RequiresGroup(ems);
        if (isRequired)
        {
            return group == null ? Json("Toto pole je povinné") : Json(true);
        }

        return Json(true);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyColour(string? colour, string ems)
    {
        bool isRequired = ems is "HCS" or "HCL";
        if (!isRequired)
        {
            return colour == null ? Json("Toto pole je povinné") : Json(true);
        }

        return Json(true);
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult VerifyBreedingBook(string? breedingBook, DateOnly dateOfBirth, bool isHomeCat)
    {
        if (!string.IsNullOrEmpty(breedingBook) ||
            dateOfBirth.AddMonths(6).ToDateTime(new TimeOnly()) >= DateTime.Now || isHomeCat)
        {
            return Json(true);
        }

        return Json("Nedomácí kočka starší než 6 měsíců potřebuje číslo plemenné knihy");
    }

    [AcceptVerbs("GET", "POST")]
    private JsonResult VerifyCageParametr(int parametr, bool selectDefault)
    {
        if (selectDefault)
        {
            return Json(true);
        }

        return parametr is < 50 or > 500 ? Json("Hodnota musí být v rozmezí 50 - 500") : Json(true);
    }

    [AcceptVerbs("GET", "POST")]
    private JsonResult VerifyEms(string ems)
    {
        Result<EmsCode> emsCode = EmsCode.Create(ems);
        if (emsCode.IsFailure)
        {
            return Json(emsCode.Error.Message);
        }

        EmsResult parsedEms = emsCode.Value.Parse();
        if (parsedEms.IsFatalFailure)
        {
            return Json(parsedEms.Error.Message);
        }

        return Json(true);
    }

    [AcceptVerbs("GET", "POST")]
    private JsonResult VerifyColour(string? colour, bool isHomeCat)
    {
        if (!string.IsNullOrEmpty(colour) || isHomeCat)
        {
            return Json(true);
        }

        return Json("Nedomácí kočka musí mít vyplněnou barvu");
    }


    [AcceptVerbs("GET", "POST")]
    private JsonResult VerifyAge(DateOnly dateOfBirth, DateOnly firstDayOfExhibition)
    {
        if (dateOfBirth.AddMonths(10) < firstDayOfExhibition)
        {
            return Json("Koťata musí být mladší než 10 měsíců");
        }

        if (dateOfBirth.AddMonths(4) >= firstDayOfExhibition)
        {
            return Json("Koťata musí být starši 4 měsíců");
        }

        return Json(true);
    }

    [AcceptVerbs("GET", "POST")]
    private JsonResult VerifyCatAge(DateOnly dateOfBirth, DateOnly firstDayOfExhibition)
    {
        if (dateOfBirth.AddMonths(4) >= firstDayOfExhibition)
        {
            return Json("Kočka musí být starši 4 měsíců");
        }

        return Json(true);
    }

    [HttpGet]
    public async Task<bool> RequiresGroup(string? ems)
    {
        bool requires = await SendQuery(new RequiresGroupQuery { Ems = ems });
        return requires;
    }

    #endregion
}
