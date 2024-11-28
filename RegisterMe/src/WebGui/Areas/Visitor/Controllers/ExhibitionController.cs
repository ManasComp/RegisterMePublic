#region

using System.Text;
using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Cages.Queries.GetRentedCageGroupById;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Exhibitions.Commands.CancelExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.CreateExhibition;
using RegisterMe.Application.Exhibitions.Commands.CreatePrices;
using RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;
using RegisterMe.Application.Exhibitions.Commands.CreateWorkflowCommand;
using RegisterMe.Application.Exhibitions.Commands.DeleteAdvertisementById;
using RegisterMe.Application.Exhibitions.Commands.DeleteDiscountCommand;
using RegisterMe.Application.Exhibitions.Commands.DeleteExhibition;
using RegisterMe.Application.Exhibitions.Commands.DeletePriceGroup;
using RegisterMe.Application.Exhibitions.Commands.DeleteRentedCages;
using RegisterMe.Application.Exhibitions.Commands.PublishExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdateAdvertisement;
using RegisterMe.Application.Exhibitions.Commands.UpdateDiscountWorkflow;
using RegisterMe.Application.Exhibitions.Commands.UpdateExhibition;
using RegisterMe.Application.Exhibitions.Commands.UpdatePaymentWorkflow;
using RegisterMe.Application.Exhibitions.Commands.UpdatePriceGroup;
using RegisterMe.Application.Exhibitions.Commands.UpdateRentedCageGroupDto;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Exhibitions.Enums;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementById;
using RegisterMe.Application.Exhibitions.Queries.GetAdvertisementsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetAllExhibitionGroupsThatAreNotFullyRegistered;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetDaysByGroupId;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountById;
using RegisterMe.Application.Exhibitions.Queries.GetDiscountsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitionById;
using RegisterMe.Application.Exhibitions.Queries.GetExhibitions;
using RegisterMe.Application.Exhibitions.Queries.GetGroupsByPriceGroupId;
using RegisterMe.Application.Exhibitions.Queries.GetPaymentsByExhibitionId;
using RegisterMe.Application.Exhibitions.Queries.GetPrices;
using RegisterMe.Application.Exhibitions.Queries.GetRentedCagesByExhibitionId;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Enums;
using RegisterMe.Application.Organizations.Queries.GetOrganizationById;
using RegisterMe.Application.Organizations.Queries.GetOrganizations;
using RegisterMe.Application.RegistrationToExhibition.Queries.ExportAllRegistrationsToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Queries.HasActiveRegistrations;
using RegisterMe.Application.Services;
using RegisterMe.Application.Services.Groups;
using RegisterMe.Application.System.Queries.PaymentByCardIsConfigured;
using RegisterMe.Application.ValueTypes;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;
using RegisterMe.Domain.Enums;
using RulesEngine.Models;
using WebGui.Areas.Visitor.Models;
using WebGui.Areas.Visitor.Models.PriceModels;
using WebGui.Services;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;

#endregion

namespace WebGui.Areas.Visitor.Controllers;

[Area(Areas.Visitor)]
public class ExhibitionController(
    UserManager<ApplicationUser> userManager,
    IMediator mediator,
    IAuthorizationService authorizationService,
    IConfiguration configuration,
    IMemoryDataService memoryDataService,
    IMapper mapper)
    : BaseController(authorizationService, mediator)
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int? organizationId, string? userId, string? searchString,
        ExhibitionRegistrationStatus exhibitionStatus = ExhibitionRegistrationStatus.CanRegisterTo, int pageNumber = 1,
        int pageSize = 10)
    {
        PaginatedList<ExhibitionDto> result = await SendQuery(new GetExhibitionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            OrganizationId = organizationId,
            UserId = userId,
            SearchString = searchString,
            ExhibitionStatus = exhibitionStatus,
            OrganizationPublishStatus = OrganizationPublishStatus.Published
        });

        List<SelectListItem> organizations = (await SendQuery(new GetOrganizationsQuery
            {
                PageNumber = 1,
                PageSize = 100,
                SearchString = string.Empty,
                OrganizationConfirmationStatus = OrganizationConfirmationStatus.Confirmed,
                HasExhibitions = null
            }))
            .Items.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
        ExhibitionModel exhibitionModel = new()
        {
            OrganizationId = organizationId,
            Organizations = organizations,
            Exhibitions = result,
            ExhibitionsFilterDto = new ExhibitionsFilterDto
            {
                OrganizationId = organizationId,
                UserId = userId,
                SearchString = searchString,
                ExhibitionRegistrationStatus = exhibitionStatus,
                OrganizationPublishStatus = OrganizationPublishStatus.All
            }
        };

        return View(exhibitionModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Detail(int id)
    {
        BriefExhibitionDto briefExhibitionDto = await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = id });
        OrganizationDto organizationDto =
            await SendQuery(new GetOrganizationByIdQuery { OrganizationId = briefExhibitionDto.OrganizationId });

        string? googleApiKey = configuration.GetValue<string>("GoogleMaps:MapApiKey");

        bool isManager;
        bool isRegistered;
        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user != null)
        {
            isManager = organizationDto.Id == user.OrganizationId;
            isRegistered =
                await SendQuery(
                    new HasActiveRegistrationsQuery { ExhibitionId = id, UserId = user.Id, Paid = true });
        }
        else
        {
            isManager = false;
            isRegistered = false;
        }

        ExhibitionDetailModel model = new()
        {
            IsRegistered = isRegistered,
            GoogleApiKey = googleApiKey,
            Status = briefExhibitionDto.Status,
            IsExhibitionManager = isManager,
            Phone = briefExhibitionDto.Phone,
            Email = briefExhibitionDto.Email,
            Name = briefExhibitionDto.Name,
            OrganizationDto = organizationDto,
            Url = briefExhibitionDto.Url,
            ExhibitionStart = briefExhibitionDto.ExhibitionStart,
            ExhibitionEnd = briefExhibitionDto.ExhibitionEnd,
            RegistrationEnd = briefExhibitionDto.RegistrationEnd,
            AddressDto = briefExhibitionDto.Address,
            Id = briefExhibitionDto.Id,
            ExhibitionId = briefExhibitionDto.Id,
            IsCancelled = briefExhibitionDto.IsCancelled,
            IsPublished = briefExhibitionDto.IsPublished
        };

        return View(model);
    }

    [HttpGet]
    public ActionResult CreateExhibition(int organizationId)
    {
        CreateExhibitionModel createExhibitionModel = CreateExhibitionModel.CreateBlank(organizationId);
        return View(createExhibitionModel);
    }

    [HttpPost]
    public async Task<ActionResult> CreateExhibition(CreateExhibitionModel createExhibitionModel)
    {
        if (!ModelState.IsValid)
        {
            return View(createExhibitionModel);
        }

        CreateExhibitionCommand createExhibitionCommand = new()
        {
            CreateExhibitionDto = new CreateExhibitionDto
            {
                Address = createExhibitionModel.Address,
                Description = createExhibitionModel.Description,
                Email = createExhibitionModel.Email,
                ExhibitionEnd = createExhibitionModel.ExhibitionEnd,
                ExhibitionStart = createExhibitionModel.ExhibitionStart,
                Name = createExhibitionModel.Name,
                BankAccount = createExhibitionModel.NormalAccount,
                OrganizationId = createExhibitionModel.OrganizationId,
                Phone = createExhibitionModel.Phone,
                RegistrationEnd = createExhibitionModel.RegistrationEnd,
                RegistrationStart = createExhibitionModel.RegistrationStart,
                Url = createExhibitionModel.Url,
                Iban = createExhibitionModel.Iban,
                DeleteNotFinishedRegistrationsAfterHours =
                    createExhibitionModel.DeleteNotFinishedRegistrationsAfterHours
            }
        };

        Result<int> exhibitionResult = await SendCommand(createExhibitionCommand, false);

        if (exhibitionResult.IsFailure)
        {
            return View(createExhibitionModel);
        }

        bool publicationCreated = (await SendCommand(new CreateAdvertisementCommand
        {
            Advertisement = new UpsertAdvertisementDto
            {
                Description = "Bez propagace", Price = new MultiCurrencyPrice(0, 0), IsDefault = true
            },
            ExhibitionId = exhibitionResult.Value
        })).IsSuccess;
        if (!publicationCreated)
        {
            TempData["Error"] = "Nepodařilo se vytvořit inzerci";
            return View(createExhibitionModel);
        }


        List<ExhibitionDayDto> exhibitionDays =
            await SendQuery(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionResult.Value });
        List<int> days = exhibitionDays.OrderBy(x => x.Date).Take(2).Select(x => x.Id).ToList();

        bool pricesCreated = await CreatePrices(exhibitionResult, days);
        if (!pricesCreated)
        {
            TempData["Error"] = "Nepodařilo se vytvořit ceník";
            return View(createExhibitionModel);
        }

        bool cagesCreated = (await SendCommand(new AddNewRentedCageGroupToExhibitionCommand
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                Count = 80,
                ExhibitionDaysId = days,
                Height = 60,
                Length = 120,
                Width = 60,
                RentedCageTypes = [RentedType.Double, RentedType.Single]
            }
        })).IsSuccess;
        if (!cagesCreated)
        {
            TempData["Error"] = "Nepodařilo se vytvořit klece";
            return View(createExhibitionModel);
        }

        bool discountsCreated = await CreateDiscounts(exhibitionResult);
        if (!discountsCreated)
        {
            TempData["Error"] = "Nepodařilo se vytvořit slevy";
            return View(createExhibitionModel);
        }

        TempData["Success"] =
            "Výstava byla úspěšně vytvořena. Pro zlehčení Vaší práce jsme ji předvyplnili. Upravte ji dle potřeb.";


        return RedirectToAction("Detail", new { id = exhibitionResult.Value });
    }

    private async Task<bool> CreateDiscounts(Result<int> exhibitionResult)
    {
        const string catRegistrationWorkflow = """
                                               [
                                                 {
                                                   "WorkflowName": "MnozstevniSleva",
                                                   "Rules": [
                                                     {
                                                       "RuleName": "MassDiscountForOneAndTwoDays",
                                                       "Expression": "catRegistration.SortedAscendingByPriceIndex >= 2",
                                                       "Actions": {
                                                         "OnSuccess": {
                                                           "Name": "OutputExpression",
                                                           "Context": {
                                                             "Expression": "utils.SetPrice(catRegistration, catRegistration.NumberOfVisitedDays == 1 ? utils.MultiCurrency(400, 20) : utils.MultiCurrency(600, 25))"
                                                           }
                                                         }
                                                       }
                                                     }
                                                   ]
                                                 },
                                                 {
                                                   "WorkflowName": "VelkaPujcenaKlec",
                                                   "Operator": "Or",
                                                   "Rules": [
                                                     {
                                                       "RuleName": "PriceForOneDayDoubleCage",
                                                       "SuccessEvent": "300,12",
                                                       "Expression": "catRegistration.CountOfUsedCagesPerRentedCageType[DoubleCageSingleCat] == 1 && !catRegistration.IsLitter"
                                                     },
                                                     {
                                                       "RuleName": "PriceForTwoDaysDoubleCage",
                                                       "SuccessEvent": "400,15",
                                                       "Expression": "catRegistration.CountOfUsedCagesPerRentedCageType[DoubleCageSingleCat]==2 && !catRegistration.IsLitter"
                                                     }
                                                   ]
                                                 },
                                                 {
                                                   "WorkflowName": "VelkaVlastniKlec",
                                                   "RuleName": "Rule1",
                                                   "Operator": "Or",
                                                   "Rules": [
                                                     {
                                                       "RuleName": "PriceForOneDayLargeCage",
                                                       "SuccessEvent": "300,12",
                                                       "Expression": "utils.FindAnyCage(catRegistration.OwnCages, 60, 60, 60, SingleCat, 1) && !catRegistration.IsLitter"
                                                     },
                                                     {
                                                       "RuleName": "PriceFOrTwoDaysLargeCage",
                                                       "SuccessEvent": "400,15",
                                                       "Expression": "utils.FindAnyCage(catRegistration.OwnCages, 60, 60, 60, SingleCat, 2) && !catRegistration.IsLitter"
                                                     }
                                                   ]
                                                 }
                                               ]

                                               """;
        IEnumerable<Workflow> catRegistrationWorkflowworkflow =
            JsonConvert.DeserializeObject<List<Workflow>>(catRegistrationWorkflow) ?? [];
        bool isOk = true;
        foreach (Workflow workflow in catRegistrationWorkflowworkflow)
        {
            isOk = (await SendCommand(new CreateWorkflowCommandCommand
            {
                Workflow = workflow, ExhibitionId = exhibitionResult.Value
            })).IsSuccess;
            if (!isOk)
            {
                break;
            }
        }

        return isOk;
    }

    private async Task<bool> CreatePrices(Result<int> exhibitionResult, List<int> days)
    {
        bool isOk = await CreatePriceForDay(exhibitionResult, days,
            ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"],
            new MultiCurrencyPrice(700, 30),
            new MultiCurrencyPrice(1100, 50)
        );
        if (!isOk)
        {
            return false;
        }

        isOk = await CreatePriceForDay(exhibitionResult, days,
            ["13a", "13b", "13c"],
            new MultiCurrencyPrice(0, 0),
            new MultiCurrencyPrice(0, 0)
        );
        if (!isOk)
        {
            return false;
        }

        isOk = await CreatePriceForDay(exhibitionResult, days,
            ["14", "15"],
            new MultiCurrencyPrice(300, 15),
            new MultiCurrencyPrice(500, 20)
        );
        if (!isOk)
        {
            return false;
        }

        isOk = await CreatePriceForDay(exhibitionResult, days,
            ["16"],
            new MultiCurrencyPrice(800, 35),
            new MultiCurrencyPrice(1400, 55)
        );
        if (!isOk)
        {
            return false;
        }

        isOk = await CreatePriceForDay(exhibitionResult, days,
            ["17"],
            new MultiCurrencyPrice(300, 25),
            new MultiCurrencyPrice(500, 40)
        );
        return isOk;
    }

    private async Task<bool> CreatePriceForDay(Result<int> exhibitionResult, List<int> days, List<string> groups,
        MultiCurrencyPrice oneDayPrice, MultiCurrencyPrice twoDayPrices)
    {
        bool isOk = (await SendCommand(new CreatePriceGroupCommand
        {
            ExhibitionId = exhibitionResult.Value,
            GroupsIds = groups,
            PriceDays =
            [
                new PriceDays { ExhibitionDayIds = days, Price = twoDayPrices }
            ]
        })).IsSuccess;
        if (!isOk)
        {
            return false;
        }

        isOk = (await SendCommand(new CreatePriceGroupCommand
        {
            ExhibitionId = exhibitionResult.Value,
            GroupsIds = groups,
            PriceDays =
            [
                new PriceDays { ExhibitionDayIds = [days[0]], Price = oneDayPrice }
            ]
        })).IsSuccess;
        if (!isOk)
        {
            return false;
        }


        isOk = (await SendCommand(new CreatePriceGroupCommand
        {
            ExhibitionId = exhibitionResult.Value,
            GroupsIds = groups,
            PriceDays =
            [
                new PriceDays { ExhibitionDayIds = [days[1]], Price = oneDayPrice }
            ]
        })).IsSuccess;
        return isOk;
    }

    [HttpGet]
    public async Task<IActionResult> EditExhibition(int id)
    {
        BriefExhibitionDto exhibition = await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = id });
        Workflow workflow = await SendQuery(new GetPaymentsByExhibitionIdQuery { ExhibitionId = id });
        IEnumerable<AdvertisementDto> advertisements =
            await SendQuery(new GetAdvertisementsByExhibitionIdQuery { ExhibitionId = id });
        IEnumerable<WorkflowDto> discounts =
            await SendQuery(new GetDiscountsByExhibitionIdQuery { ExhibitionId = id });
        List<BigPriceDto> prices = await SendQuery(new GetPricesQuery { ExhibitionId = id });
        List<BriefCageDto> cages =
            await SendQuery(new GetRentedCagesByExhibitionIdQuery { ExhibitionId = id });
        bool canPayByCard = await SendQuery(new PaymentByCardIsConfiguredQuery());
        List<DatabaseGroupDto> getGroupsThatAreNotFullyRegistered =
            await SendQuery(new GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery { ExhibitionId = id });
        bool isFullyRegistered = getGroupsThatAreNotFullyRegistered.Count == 0;
        EditExhibitionModel model = new()
        {
            Exhibition = exhibition,
            PaymentTypes = workflow,
            Advertisements = advertisements.ToList(),
            Discounts = discounts.ToList(),
            Prices = prices.ToList(),
            Cages = cages,
            CanPayByCard = canPayByCard,
            IsFullyRegistered = isFullyRegistered
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportData(int exhibitionId)
    {
        string json = await SendQuery(new ExportAllRegistrationsToExhibitionQuery { ExhibitionId = exhibitionId });
        return File(Encoding.UTF8.GetBytes(json), "application/json", "data.json");
    }

    [HttpGet]
    public async Task<ActionResult> EditExhibitionData(int exhibitionId)
    {
        BriefExhibitionDto exhibition = await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = exhibitionId });
        UpdateExhibitionDto createExhibitionModel = mapper.Map<UpdateExhibitionDto>(exhibition);
        return View(createExhibitionModel);
    }

    [HttpPost]
    public async Task<ActionResult> EditExhibitionData(UpdateExhibitionDto updateExhibitionDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateExhibitionDto);
        }

        UpdateExhibitionCommand createExhibitionCommand = new()
        {
            UpdateExhibitionDto = new UpdateExhibitionDto
            {
                Id = updateExhibitionDto.Id,
                Address = updateExhibitionDto.Address,
                Description = updateExhibitionDto.Description,
                Email = updateExhibitionDto.Email,
                ExhibitionEnd = updateExhibitionDto.ExhibitionEnd,
                ExhibitionStart = updateExhibitionDto.ExhibitionStart,
                Name = updateExhibitionDto.Name,
                BankAccount = updateExhibitionDto.BankAccount,
                Phone = updateExhibitionDto.Phone,
                RegistrationEnd = updateExhibitionDto.RegistrationEnd,
                RegistrationStart = updateExhibitionDto.RegistrationStart,
                Url = updateExhibitionDto.Url,
                Iban = updateExhibitionDto.Iban,
                DeleteNotFinishedRegistrationsAfterHours =
                    updateExhibitionDto.DeleteNotFinishedRegistrationsAfterHours
            }
        };

        Result result = await SendCommand(createExhibitionCommand, false);
        if (result.IsFailure)
        {
            return View(updateExhibitionDto);
        }

        return RedirectToAction("Detail", new { id = updateExhibitionDto.Id });
    }

    [HttpGet]
    public async Task<ActionResult> UpsertAdvertisement(int exhibitionId, int? advertisementId)
    {
        AdvertisementModel advertisementModel;
        if (advertisementId.HasValue)
        {
            AdvertisementDto advertisementDto =
                await SendQuery(new GetAdvertisementByIdQuery { AdvertisementId = advertisementId.Value });
            advertisementModel = AdvertisementModel.CreateFromDto(exhibitionId, advertisementDto);
            return View(advertisementModel);
        }

        advertisementModel = AdvertisementModel.CreateBlank(exhibitionId);
        return View(advertisementModel);
    }

    [HttpPost]
    public async Task<ActionResult> UpsertAdvertisement(AdvertisementModel advertisementModel)
    {
        if (!ModelState.IsValid)
        {
            return View(advertisementModel);
        }

        UpsertAdvertisementDto advertisement = new()
        {
            Description = advertisementModel.Description,
            Price = new MultiCurrencyPrice(advertisementModel.PriceKc, advertisementModel.PriceEur),
            IsDefault = advertisementModel.IsDefault
        };
        if (advertisementModel.Id.HasValue)
        {
            Result result =
                await SendCommand(
                    new UpdateAdvertisementCommand
                    {
                        AdvertisementDto = advertisement, AdvertisementId = advertisementModel.Id.Value
                    }, false);
            if (result.IsFailure)
            {
                return View(advertisementModel);
            }

            return RedirectToAction("EditExhibition", new { id = advertisementModel.ExhibitionId });
        }

        Result<int> result1 =
            await SendCommand(
                new CreateAdvertisementCommand
                {
                    Advertisement = advertisement, ExhibitionId = advertisementModel.ExhibitionId
                }, false);
        if (result1.IsFailure)
        {
            return View(advertisementModel);
        }

        return RedirectToAction("EditExhibition", new { id = advertisementModel.ExhibitionId });
    }

    [HttpGet]
    public async Task<ActionResult> UpsertPriceStep1(int exhibitionId, string? pricesIds, bool isFirs = false)
    {
        if (isFirs)
        {
            memoryDataService.DeleteTemporaryPrice(exhibitionId);
        }

        TemporaryPriceDto? temporaryPrice = await memoryDataService.GetTemporaryPrice(exhibitionId);
        if (temporaryPrice != null)
        {
            PriceModelStep1 priceModel1 =
                new() { ExhibitionId = exhibitionId, AvailableGroups = temporaryPrice.Groups };

            return View(priceModel1);
        }

        List<DatabaseGroupDto> groups = await SendQuery(
            new GetAllExhibitionGroupsThatAreNotFullyRegisteredQuery { ExhibitionId = exhibitionId });

        List<StringCheckBox> availableGroupsAvailableGroups = groups.Select(x =>
            new StringCheckBox { Id = x.GroupId, LabelName = x.Name, IsChecked = false }).ToList();

        if (pricesIds != null)
        {
            List<DatabaseGroupDto> newGroups =
                await SendQuery(new GetGroupsByPriceGroupIdQuery { GroupId = pricesIds });
            foreach (DatabaseGroupDto group in newGroups)
            {
                StringCheckBox? avaGroup = availableGroupsAvailableGroups.Find(x => x.Id == group.GroupId);
                if (avaGroup != null)
                {
                    avaGroup.IsChecked = true;
                }
                else
                {
                    availableGroupsAvailableGroups.Add(new StringCheckBox
                    {
                        Id = group.GroupId, LabelName = group.Name, IsChecked = true
                    });
                }
            }
        }

        availableGroupsAvailableGroups = availableGroupsAvailableGroups.OrderBy(x => new GroupId(x.Id)).ToList();

        if (availableGroupsAvailableGroups.Count == 0)
        {
            TempData["Error"] = "Nejsou už žádné skupiny, pro které by nebyla cena již přiřazena";
            return RedirectToAction("EditExhibition", new { id = exhibitionId });
        }

        PriceModelStep1 priceModel = new()
        {
            ExhibitionId = exhibitionId, AvailableGroups = availableGroupsAvailableGroups
        };

        memoryDataService.SetTemporaryPrice(
            new TemporaryPriceDto { Groups = priceModel.AvailableGroups, OriginalPricesIds = pricesIds },
            priceModel.ExhibitionId);

        return View(priceModel);
    }

    [HttpPost]
    public ActionResult UpsertPriceStep1(PriceModelStep1 priceModel)
    {
        bool atLeastOneGroupSelected = priceModel.AvailableGroups.Select(x => x.IsChecked).Any(x => x);
        if (!atLeastOneGroupSelected)
        {
            ModelState.AddModelError(CommandValidation, "Musíte vybrat alespoň jednu skupinu.");
        }

        if (!ModelState.IsValid)
        {
            return View(priceModel);
        }

        TemporaryPriceDto? temporaryPrice =
            memoryDataService.GetTemporaryPrice(priceModel.ExhibitionId).Result;
        Guard.Against.Null(temporaryPrice, nameof(temporaryPrice));
        memoryDataService.SetTemporaryPrice(
            new TemporaryPriceDto
            {
                Groups = priceModel.AvailableGroups, OriginalPricesIds = temporaryPrice.OriginalPricesIds
            }, priceModel.ExhibitionId);

        return RedirectToAction("UpsertPriceStep2", new { exhibitionId = priceModel.ExhibitionId });
    }

    [HttpGet]
    public async Task<ActionResult> UpsertPriceStep2(int exhibitionId)
    {
        TemporaryPriceDto? temporaryPrice = await memoryDataService.GetTemporaryPrice(exhibitionId);
        if (temporaryPrice == null)
        {
            return RedirectToAction("UpsertPriceStep1", new { exhibitionId });
        }

        if (temporaryPrice.Days.Count != 0)
        {
            PriceModelStep2 priceModel1 = new()
            {
                ExhibitionId = exhibitionId,
                Days = temporaryPrice.Days,
                PricesIds = temporaryPrice.OriginalPricesIds
            };

            return View(priceModel1);
        }

        List<ExhibitionDayDto> days =
            await SendQuery(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId });
        PriceModelStep2 priceModel = new()
        {
            ExhibitionId = exhibitionId,
            Days = days.Select(x =>
                    new IntCheckBox { IsChecked = false, Id = x.Id, LabelName = x.Date.ToString("dd.MM.yyyy") })
                .ToList(),
            PricesIds = temporaryPrice.OriginalPricesIds
        };
        if (temporaryPrice.OriginalPricesIds == null)
        {
            return View(priceModel);
        }

        {
            HashSet<int> currentDays =
                (await SendQuery(new GetDaysByGroupIdQuery { PriceGroupIds = temporaryPrice.OriginalPricesIds }))
                .Select(x => x.Id).ToHashSet();
            foreach (IntCheckBox day in priceModel.Days.Where(day => currentDays.Contains(day.Id)))
            {
                day.IsChecked = true;
            }
        }

        return View(priceModel);
    }

    [HttpPost]
    public ActionResult UpsertPriceStep2(PriceModelStep2 priceModel)
    {
        bool atLeastOneDaySelected = priceModel.Days.Select(x => x.IsChecked).Any(x => x);
        if (!atLeastOneDaySelected)
        {
            ModelState.AddModelError(CommandValidation, "Musíte vybrat alespoň jeden den.");
        }

        if (!ModelState.IsValid)
        {
            return View(priceModel);
        }

        TemporaryPriceDto? temporaryPrice =
            memoryDataService.GetTemporaryPrice(priceModel.ExhibitionId).Result;
        if (temporaryPrice == null)
        {
            return RedirectToAction("UpsertPriceStep1", new { exhibitionId = priceModel.ExhibitionId });
        }

        memoryDataService.DeleteTemporaryPrice(priceModel.ExhibitionId);
        memoryDataService.SetTemporaryPrice(
            new TemporaryPriceDto
            {
                Groups = temporaryPrice.Groups,
                Days = priceModel.Days,
                OriginalPricesIds = temporaryPrice.OriginalPricesIds
            },
            priceModel.ExhibitionId);
        return RedirectToAction("UpsertPriceStep3", new { exhibitionId = priceModel.ExhibitionId });
    }

    [HttpGet]
    public async Task<ActionResult> UpsertPriceStep3(int exhibitionId)
    {
        TemporaryPriceDto? temporaryPrice = await memoryDataService.GetTemporaryPrice(exhibitionId);
        if (temporaryPrice == null)
        {
            return RedirectToAction("UpsertPriceStep1", new { exhibitionId });
        }

        List<List<int>> allDayCombinations =
            MathHelper.GetCombinationsOfGivenList(
                temporaryPrice.Days.Where(x => x.IsChecked).Select(x => x.Id).ToList());

        var exhibitionDays = (await SendQuery(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId }))
            .Select(x => new { id = x.Id, date = x.Date }).ToHashSet();

        List<ExhibitionDaysWithPrice> priceDays = allDayCombinations.Select(x =>
            new ExhibitionDaysWithPrice
            {
                ExhibitionDays =
                    x.Select(y =>
                            new SmallExhibitionDayDto { Id = y, Date = exhibitionDays.Single(arg => arg.id == y).date })
                        .ToList(),
                PriceCzk = 100,
                PriceEur = 10
            }).ToList();
        PriceModelStep3 priceModel = new() { ExhibitionId = exhibitionId, PriceDays = priceDays };
        return View(priceModel);
    }

    [HttpPost]
    public async Task<ActionResult> UpsertPriceStep3(PriceModelStep3 priceModel)
    {
        if (!ModelState.IsValid)
        {
            return View(priceModel);
        }

        Result? result;
        TemporaryPriceDto? temporaryPrice =
            await memoryDataService.GetTemporaryPrice(priceModel.ExhibitionId);
        if (temporaryPrice == null)
        {
            return RedirectToAction("UpsertPriceStep1", new { exhibitionId = priceModel.ExhibitionId });
        }

        if (temporaryPrice.OriginalPricesIds == null)
        {
            CreatePriceGroupCommand createPriceGroupCommand = new()
            {
                ExhibitionId = priceModel.ExhibitionId,
                GroupsIds = temporaryPrice.Groups.Where(x => x.IsChecked).Select(x => x.Id).ToList(),
                PriceDays = priceModel.PriceDays.Select(x =>
                        new PriceDays
                        {
                            ExhibitionDayIds = x.ExhibitionDays.Select(y => y.Id).ToList(),
                            Price = new MultiCurrencyPrice(x.PriceCzk, x.PriceEur)
                        })
                    .ToList()
            };
            result = await SendCommand(createPriceGroupCommand, false);
        }
        else
        {
            UpdatePriceGroupCommand updatePricesCommand = new()
            {
                GroupsIds = temporaryPrice.Groups.Where(x => x.IsChecked).Select(x => x.Id).ToList(),
                PriceDays = priceModel.PriceDays.Select(x =>
                        new PriceDays
                        {
                            ExhibitionDayIds = x.ExhibitionDays.Select(y => y.Id).ToList(),
                            Price = new MultiCurrencyPrice(x.PriceCzk, x.PriceEur)
                        })
                    .ToList(),
                OriginalPricesId = temporaryPrice.OriginalPricesIds
            };
            result = await SendCommand(updatePricesCommand, false, "Ceník byl úspěšně aktualizován.");
        }

        if (result.IsFailure)
        {
            return View(priceModel);
        }

        memoryDataService.DeleteTemporaryPrice(priceModel.ExhibitionId);
        return RedirectToAction("EditExhibition", new { id = priceModel.ExhibitionId });
    }

    [HttpPost]
    public async Task<ActionResult> DeleteAdvertisement(DeleteAdvertisementModel deleteAdvertisementModel)
    {
        await SendCommand(
            new DeleteAdvertisementCommand { AdvertisementId = deleteAdvertisementModel.AdvertisementId });
        return RedirectToAction("EditExhibition", new { id = deleteAdvertisementModel.ExhibitionId });
    }

    [HttpPost]
    public async Task<ActionResult> DeletePriceGroup(DeletePriceGroupModel command)
    {
        await SendCommand(new DeletePriceGroupCommand { PriceIds = command.PriceIds });
        return RedirectToAction("EditExhibition", new { id = command.ExhibitionId });
    }

    [HttpGet]
    public async Task<ActionResult> UpdatePaymentMethods(int exhibitionId)
    {
        Workflow workflow = await SendQuery(new GetPaymentsByExhibitionIdQuery { ExhibitionId = exhibitionId });
        bool canPayByCard = await SendQuery(new PaymentByCardIsConfiguredQuery());
        UpdatePaymentMethodsModel model = new()
        {
            ExhibitionId = exhibitionId, PaymentTypes = workflow, CanPayByCard = canPayByCard
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> UpdatePaymentMethods(UpdatePaymentMethodsModel model)
    {
        Workflow? workflow = null;
        try
        {
            workflow = JsonConvert.DeserializeObject<Workflow>(model.PaymentTypesString);
        }
        catch (Exception)
        {
            ModelState.AddModelError("PaymentTypesString", "Invalid json");
        }

        if (workflow == null)
        {
            ModelState.AddModelError("PaymentTypesString", "Invalid json");
        }

        if (!ModelState.IsValid)
        {
            bool canPayByCard = await SendQuery(new PaymentByCardIsConfiguredQuery());
            Workflow newWorkflow =
                await SendQuery(new GetPaymentsByExhibitionIdQuery { ExhibitionId = model.ExhibitionId });
            UpdatePaymentMethodsModel model1 = new()
            {
                ExhibitionId = model.ExhibitionId, PaymentTypes = newWorkflow, CanPayByCard = canPayByCard
            };
            return View(model1);
        }

        Guard.Against.Null(workflow);
        Result result = await SendCommand(
            new UpdatePaymentWorkflowCommand { ExhibitionId = model.ExhibitionId, PaymentWorkflow = workflow });
        if (result.IsFailure)
        {
            bool canPayByCard = await SendQuery(new PaymentByCardIsConfiguredQuery());
            UpdatePaymentMethodsModel model1 = new()
            {
                ExhibitionId = model.ExhibitionId, PaymentTypes = workflow, CanPayByCard = canPayByCard
            };
            return View(model1);
        }

        return RedirectToAction("EditExhibition", new { id = model.ExhibitionId });
    }

    [HttpGet]
    public async Task<ActionResult> UpsertDiscount(int exhibitionId, int? discountId)
    {
        UpdateDiscountMethodsModel model;
        if (discountId == null)
        {
            const string workflowJson = """
                                        {
                                                            "WorkflowName": "WorkflowName",
                                                            "Rules": [
                                                            {
                                                            "RuleName": "RuleName",
                                                            "Expression": "true",
                                                            "Actions": {
                                                            "OnSuccess": {
                                                            "Name": "OutputExpression",
                                                            "Context": {
                                                            "Expression": "0"
                                                            }
                                                            },
                                                            }
                                                            }                 
                                                            ]
                                                        }
                                        """;

            Workflow? defaultWorkflowCatRegistrationWorkflow =
                JsonConvert.DeserializeObject<Workflow>(workflowJson);
            Guard.Against.Null(defaultWorkflowCatRegistrationWorkflow);
            model = new UpdateDiscountMethodsModel
            {
                ExhibitionId = exhibitionId,
                DiscountWorkflow = defaultWorkflowCatRegistrationWorkflow,
                DiscountId = null
            };
        }
        else
        {
            Workflow workflow = await SendQuery(new GetDiscountByIdQuery { WorkflowId = discountId.Value });

            model = new UpdateDiscountMethodsModel
            {
                ExhibitionId = exhibitionId, DiscountWorkflow = workflow, DiscountId = discountId
            };
        }

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> UpsertDiscount(UpdateDiscountMethodsModel model)
    {
        WorkflowDto? workflow = null;
        try
        {
            workflow = JsonConvert.DeserializeObject<WorkflowDto>(model.DiscountWorkflowString);
        }
        catch (Exception)
        {
            ModelState.AddModelError("DiscountWorkflowString", "Invalid json");
        }

        if (workflow == null)
        {
            ModelState.AddModelError("DiscountWorkflowString", "Invalid json");
        }

        if (!ModelState.IsValid)
        {
            Workflow newWorkflow = await SendQuery(new GetDiscountByIdQuery { WorkflowId = model.ExhibitionId });
            UpdateDiscountMethodsModel model1 = new()
            {
                DiscountId = model.DiscountId, ExhibitionId = model.ExhibitionId, DiscountWorkflow = newWorkflow
            };
            return View(model1);
        }

        Guard.Against.Null(workflow);
        Result result;
        if (model.DiscountId == null)
        {
            result = await SendCommand(
                new CreateWorkflowCommandCommand { ExhibitionId = model.ExhibitionId, Workflow = workflow });
        }
        else
        {
            result =
                await SendCommand(
                    new UpdateDiscountWorkflowCommand { Id = model.DiscountId.Value, Workflow = workflow });
        }

        if (result.IsFailure)
        {
            UpdateDiscountMethodsModel model1 = new()
            {
                DiscountId = model.DiscountId, ExhibitionId = model.ExhibitionId, DiscountWorkflow = workflow
            };
            return View(model1);
        }

        return RedirectToAction("EditExhibition", new { id = model.ExhibitionId });
    }

    [HttpPost]
    public async Task<ActionResult> DeleteDiscount(DeleteDiscountModel deleteDiscountModel)
    {
        await SendCommand(new DeleteDiscountCommand { Id = deleteDiscountModel.DiscountId });
        return RedirectToAction("EditExhibition", new { id = deleteDiscountModel.ExhibitionId });
    }

    [HttpGet]
    public async Task<ActionResult> UpsertRentedCages(int exhibitionId, string? cagesId)
    {
        List<ExhibitionDayDto> exhibitionDaysDto =
            await SendQuery(new GetDaysByExhibitionIdQuery { ExhibitionId = exhibitionId });
        IEnumerable<RentedType> rentedTypes = Enum.GetValues(typeof(RentedType)).Cast<RentedType>().ToList();

        CreateCageModel model = new()
        {
            ExhibitionId = exhibitionId,
            Length = 120,
            Width = 60,
            Height = 60,
            Count = 10,
            ExhibitionDaysId = CreateExhibitionDaysCheckboxes(exhibitionDaysDto, null),
            RentedCageTypes = CreateRentedTypesCheckboxes(rentedTypes, null)
        };

        if (cagesId != null)
        {
            BriefCageDto cages = await SendQuery(new GetRentedCageGroupByIdQuery { CagesId = cagesId });
            model.Length = cages.Length;
            model.Width = cages.Width;
            model.Height = cages.Height;
            model.Count = cages.Count;
            model.CagesIds = cagesId;
            model.ExhibitionDaysId =
                CreateExhibitionDaysCheckboxes(exhibitionDaysDto, cages.ExhibitionDays.Select(x => x.Id).ToList());
            model.RentedCageTypes = CreateRentedTypesCheckboxes(rentedTypes, cages.RentedTypes);
        }

        return View(model);
    }

    private List<IntCheckBox> CreateExhibitionDaysCheckboxes(List<ExhibitionDayDto> exhibitionDays,
        List<int>? selectedDays)
    {
        return exhibitionDays.Select(x => new IntCheckBox
        {
            Id = x.Id,
            LabelName = x.Date.ToString("dd.MM.yyyy"),
            IsChecked = selectedDays?.Any(y => y == x.Id) ?? true
        }).ToList();
    }

    private List<IntCheckBox> CreateRentedTypesCheckboxes(IEnumerable<RentedType> rentedTypes,
        List<RentedType>? selectedTypes)
    {
        return rentedTypes.Select(x => new IntCheckBox
        {
            Id = (int)x, LabelName = GetRentedTypeAsString(x), IsChecked = selectedTypes?.Contains(x) ?? true
        }).ToList();
    }

    public static string GetRentedTypeAsString(RentedType rentedType)
    {
        return rentedType switch
        {
            RentedType.Double => "Dvojitá",
            RentedType.Single => "Jednoduchá",
            _ => throw new ArgumentOutOfRangeException(nameof(rentedType), rentedType, null)
        };
    }

    [HttpPost]
    public async Task<ActionResult> UpsertRentedCages(CreateCageModel model)
    {
        List<RentedType> rentedTypes =
            model.RentedCageTypes.Where(x => x.IsChecked).Select(x => (RentedType)x.Id).ToList();
        List<int> exhibitionDays = model.ExhibitionDaysId.Where(x => x.IsChecked).Select(x => x.Id).ToList();

        if (!ModelState.IsValid)
        {
            List<ExhibitionDayDto> exhibitionDaysDto =
                await SendQuery(new GetDaysByExhibitionIdQuery { ExhibitionId = model.ExhibitionId });
            IEnumerable<RentedType> allRentedTypes = Enum.GetValues(typeof(RentedType)).Cast<RentedType>().ToList();
            model.ExhibitionDaysId = CreateExhibitionDaysCheckboxes(exhibitionDaysDto, exhibitionDays);
            model.RentedCageTypes = CreateRentedTypesCheckboxes(allRentedTypes, rentedTypes);
            return View(model);
        }

        if (model.CagesIds != null)
        {
            Result<string> result = await SendCommand(new UpdateRentedCageGroupCommand
            {
                CagesId = model.CagesIds,
                RentedCage = new CreateRentedRentedCageDto
                {
                    ExhibitionDaysId = exhibitionDays,
                    Length = model.Length,
                    Width = model.Width,
                    Height = model.Height,
                    RentedCageTypes = rentedTypes,
                    Count = model.Count
                }
            });
            if (result.IsFailure)
            {
                List<ExhibitionDayDto> exhibitionDaysDto =
                    await SendQuery(new GetDaysByExhibitionIdQuery { ExhibitionId = model.ExhibitionId });
                IEnumerable<RentedType> allRentedTypes = Enum.GetValues(typeof(RentedType)).Cast<RentedType>().ToList();
                model.ExhibitionDaysId = CreateExhibitionDaysCheckboxes(exhibitionDaysDto, exhibitionDays);
                model.RentedCageTypes = CreateRentedTypesCheckboxes(allRentedTypes, rentedTypes);
                return View(model);
            }

            return RedirectToAction("EditExhibition", new { id = model.ExhibitionId });
        }

        AddNewRentedCageGroupToExhibitionCommand createRentedCageDto = new()
        {
            CreateRentedRentedCageDto = new CreateRentedRentedCageDto
            {
                ExhibitionDaysId = exhibitionDays,
                Length = model.Length,
                Width = model.Width,
                Height = model.Height,
                RentedCageTypes = rentedTypes,
                Count = model.Count
            }
        };
        Result<string> resutl = await SendCommand(createRentedCageDto, false);
        if (resutl.IsFailure)
        {
            return View(model);
        }

        return RedirectToAction("EditExhibition", new { id = model.ExhibitionId });
    }

    [HttpPost]
    public async Task<ActionResult> DeleteCages(DeleteCagesCommand deleteCagesCommand)
    {
        await SendCommand(new DeleteRentedCagesCommand { CagesId = deleteCagesCommand.CagesId });
        return RedirectToAction("EditExhibition", new { id = deleteCagesCommand.ExhibitionId });
    }

    [HttpPost]
    public async Task<ActionResult> PublishExhibition(PublishExhibitionDto deleteExibitionDto)
    {
        await SendCommand(new PublishExhibitionCommand { ExhibitionId = deleteExibitionDto.ExhibitionId });
        return RedirectToAction("EditExhibition", new { id = deleteExibitionDto.ExhibitionId });
    }

    [HttpPost]
    public async Task<ActionResult> DeleteOrCancelExhibition(DeleteExhibitionDto deleteExhibitionDto)
    {
        BriefExhibitionDto exhibition =
            await SendQuery(new GetExhibitionByIdQuery { ExhibitionId = deleteExhibitionDto.ExhibitionId });
        if (!exhibition.IsPublished)
        {
            await SendCommand(
                new DeleteUnpublishedExhibitionCommand { ExhibitionId = deleteExhibitionDto.ExhibitionId });
        }
        else
        {
            await SendCommand(new CancelExhibitionCommand { ExhibitionId = deleteExhibitionDto.ExhibitionId });
        }

        return RedirectToAction(nameof(AdministratorController.Index), "Administrator");
    }

    [HttpGet]
    public ActionResult Info()
    {
        string? superAdministratorMail = configuration.GetValue<string>("Email:Mail");
        Guard.Against.NullOrWhiteSpace(superAdministratorMail);
        return View(new EmailModel { Email = superAdministratorMail });
    }
}
