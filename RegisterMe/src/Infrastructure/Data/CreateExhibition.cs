#region

using Newtonsoft.Json;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;
using RegisterMe.Domain.Enums;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Infrastructure.Data;

public static class CreateExhibition
{
    public static async Task CreateXvExhibition(ApplicationDbContext context,
        TimeProvider dateTime, List<Group> groups, bool published, int organizationId)
    {
        Exhibition exhibition = Exhibition.CreateInstance();
        exhibition.DeleteNotFinishedRegistrationsAfterHours = 24;
        exhibition.Name = "XV. and XVI. International Cat Show";
        exhibition.Url = "https://www.schk.cz/files/Ko-kyBrno2023_eng.pdf";
        exhibition.Description = "2 certifikáty, všechny skupiny";
        exhibition.RegistrationStart = DateOnly.FromDateTime(dateTime.GetUtcNow().AddDays(-5).DateTime);
        exhibition.RegistrationEnd = DateOnly.FromDateTime(dateTime.GetUtcNow().AddDays(5).DateTime);
        exhibition.Phone = "+420 605 262 990";
        exhibition.Email = "525025@muni.cz"; // todo change in production
        exhibition.Iban = "CZ1320100000002600780927";
        exhibition.BankAccount = "2600780927/2010";
        exhibition.IsPublished = published;
        exhibition.OrganizationId = organizationId;
        exhibition.Address = new Address
        {
            Latitude = "49.20994936127367",
            Longitude = "16.598969230293026",
            StreetAddress = "Botanická 68A, 602 00 Brno-Královo Pole-Ponava"
        };
        context.Exhibitions.Add(exhibition);

        await context.SaveChangesAsync();
        ExhibitionDay day1 = new()
        {
            Date = DateOnly.FromDateTime(dateTime.GetUtcNow().AddDays(6).DateTime), ExhibitionId = exhibition.Id
        };
        ExhibitionDay day2 = new()
        {
            Date = DateOnly.FromDateTime(dateTime.GetUtcNow().AddDays(7).DateTime), ExhibitionId = exhibition.Id
        };
        context.ExhibitionDays.Add(day1);
        context.ExhibitionDays.Add(day2);
        context.Advertisements.Add(new Advertisement
        {
            Description = "A5 propagace",
            ExhibitionId = exhibition.Id,
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            IsDefault = false
        });
        context.Advertisements.Add(new Advertisement
        {
            Description = "A4 propagace",
            ExhibitionId = exhibition.Id,
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 500 },
                new Amounts { Currency = Currency.Eur, Amount = 45 }
            ],
            IsDefault = false
        });
        context.Advertisements.Add(new Advertisement
        {
            Description = "Bez propagace",
            ExhibitionId = exhibition.Id,
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            IsDefault = true
        });
        await context.SaveChangesAsync();

        List<RentedCage> list = [];
        for (int i = 0; i < 3; i++)
        {
            list.Add(new RentedCage
            {
                Length = 120,
                Width = 60,
                Height = 60,
                ExhibitionDays = [day1, day2],
                RentedTypes =
                [
                    new RentedTypeEntity { RentedType = RentedType.Single },
                    new RentedTypeEntity { RentedType = RentedType.Double }
                ]
            });
        }

        context.RentedCages.AddRange(list);
        await context.SaveChangesAsync();

        List<Group> groups_1_3_5_7_9 = groups.Where(x =>
                x.GroupId is "1" or "3" or "5" or "7" or "9")
            .ToList();
        Price price1_1_3_5_7_9 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            Groups = groups_1_3_5_7_9,
            ExhibitionDays = { day1 }
        };
        Price price2_1_3_5_7_9 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1200 },
                new Amounts { Currency = Currency.Eur, Amount = 50 }
            ],
            Groups = groups_1_3_5_7_9,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_1_3_5_7_9 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            Groups = groups_1_3_5_7_9,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_1_3_5_7_9);
        context.Prices.Add(price2_1_3_5_7_9);
        context.Prices.Add(price3_1_3_5_7_9);

        List<Group> groups_2_4_6_8_10_11_12 = groups.Where(x =>
            x.GroupId is "2" or "4" or "6" or "8" or "10" or "11" or "12").ToList();
        List<Group> groups_15 = groups.Where(x =>
            x.GroupId == "15").ToList();
        List<Group> groups_16 = groups.Where(x =>
            x.GroupId == "16").ToList();
        List<Group> groups_17 = groups.Where(x =>
            x.GroupId == "17").ToList();
        Price price1_2_4_6_8_10_11_12 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            Groups = groups_2_4_6_8_10_11_12,
            ExhibitionDays = { day1 }
        };
        Price price2_2_4_6_8_10_11_12 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1200 },
                new Amounts { Currency = Currency.Eur, Amount = 50 }
            ],
            Groups = groups_2_4_6_8_10_11_12,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_2_4_6_8_10_11_12 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            Groups = groups_2_4_6_8_10_11_12,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_2_4_6_8_10_11_12);
        context.Prices.Add(price2_2_4_6_8_10_11_12);
        context.Prices.Add(price3_2_4_6_8_10_11_12);

        List<Group> groups_13 = groups.Where(x => x.GroupId is "13a" or "13b" or "13c")
            .ToList();
        Price price1_13 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            Groups = groups_13,
            ExhibitionDays = { day1 }
        };
        Price price2_13 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            Groups = groups_13,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_13 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            Groups = groups_13,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_13);
        context.Prices.Add(price2_13);
        context.Prices.Add(price3_13);

        List<Group> groups_14 = groups.Where(x => x.GroupId == "14").ToList();
        Price price1_14 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 600 },
                new Amounts { Currency = Currency.Eur, Amount = 25 }
            ],
            Groups = groups_14,
            ExhibitionDays = { day1 }
        };
        Price price2_14 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1000 },
                new Amounts { Currency = Currency.Eur, Amount = 40 }
            ],
            Groups = groups_14,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_14 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 600 },
                new Amounts { Currency = Currency.Eur, Amount = 25 }
            ],
            Groups = groups_14,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_14);
        context.Prices.Add(price2_14);
        context.Prices.Add(price3_14);

        Price price1_15 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 600 },
                new Amounts { Currency = Currency.Eur, Amount = 10 }
            ],
            Groups = groups_15,
            ExhibitionDays = { day1 }
        };
        Price price2_15 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1000 },
                new Amounts { Currency = Currency.Eur, Amount = 10 }
            ],
            Groups = groups_15,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_15 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 600 },
                new Amounts { Currency = Currency.Eur, Amount = 10 }
            ],
            Groups = groups_15,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_15);
        context.Prices.Add(price2_15);
        context.Prices.Add(price3_15);

        Price price1_16 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 800 },
                new Amounts { Currency = Currency.Eur, Amount = 35 }
            ],
            Groups = groups_16,
            ExhibitionDays = { day1 }
        };
        Price price2_16 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1400 },
                new Amounts { Currency = Currency.Eur, Amount = 60 }
            ],
            Groups = groups_16,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_16 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 800 },
                new Amounts { Currency = Currency.Eur, Amount = 35 }
            ],
            Groups = groups_16,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_16);
        context.Prices.Add(price2_16);
        context.Prices.Add(price3_16);

        Price price1_17 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 400 },
                new Amounts { Currency = Currency.Eur, Amount = 25 }
            ],
            Groups = groups_17,
            ExhibitionDays = { day1 }
        };
        Price price2_17 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 40 }
            ],
            Groups = groups_17,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_17 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 400 },
                new Amounts { Currency = Currency.Eur, Amount = 25 }
            ],
            Groups = groups_17,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_17);
        context.Prices.Add(price2_17);
        context.Prices.Add(price3_17);

        const string PaymentTypes = """
                                    [
                                      {
                                        "WorkflowName": "Payment",
                                        "Rules": [
                                          {
                                            "RuleName": "PayByBankTransfer_CZK",
                                            "Expression": "registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                          },
                                          {
                                            "RuleName": "PayOnlineByCard_CZK",
                                            "Expression": "false"
                                          },
                                          {
                                            "RuleName": "PayInPlaceByCache_CZK",
                                            "Expression": "registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                          },
                                          {
                                            "RuleName": "PayByBankTransfer_EUR",
                                            "Expression": "!registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                          },
                                          {
                                            "RuleName": "PayOnlineByCard_EUR",
                                            "Expression": "false"
                                          },
                                          {
                                            "RuleName": "PayInPlaceByCache_EUR",
                                            "Expression": "!registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                          }
                                        ]
                                      }
                                    ]
                                    """;

        PriceTypeWorkflow paymentTypes =
            (JsonConvert.DeserializeObject<List<Workflow>>(PaymentTypes) ?? [])
            .Select(x => new PriceTypeWorkflow(x, exhibition.Id)).First();
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
                                                             "Expression": "utils.SetPrice(catRegistration, catRegistration.NumberOfVisitedDays == 1 ? utils.MultiCurrency(600, 20) : utils.MultiCurrency(900, 40))"
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
                                                       "SuccessEvent": "600,24",
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
                                                       "SuccessEvent": "600,24",
                                                       "Expression": "utils.FindAnyCage(catRegistration.OwnCages, 60, 60, 60, SingleCat, 2) && !catRegistration.IsLitter"
                                                     }
                                                   ]
                                                 }
                                               ]

                                               """;
        IEnumerable<PriceAdjustmentWorkflow> catRegistrationWorkflows =
            (JsonConvert.DeserializeObject<List<Workflow>>(catRegistrationWorkflow) ?? [])
            .Select(x => new PriceAdjustmentWorkflow(x, exhibition.Id));

        context.PriceAdjustmentWorkflows.AddRange(catRegistrationWorkflows);
        context.PriceTypeWorkflows.Add(paymentTypes);
        await context.SaveChangesAsync();
    }

    public static async Task CreateXviiExhibition(ApplicationDbContext context, List<Group> groups, bool published,
        int organizationId)
    {
        Exhibition exhibition = Exhibition.CreateInstance();
        exhibition.DeleteNotFinishedRegistrationsAfterHours = 24;
        exhibition.Name = "XVII. a XVIII. Mezinárodní výstava koček";
        exhibition.Url = "https://www.schk.cz/files/pozvanka2024.pdf";
        exhibition.Description = "2 certifikáty, všechny skupiny";
        exhibition.RegistrationStart = new DateOnly(2024, 9, 1);
        exhibition.RegistrationEnd = new DateOnly(2024, 10, 25);
        exhibition.Phone = "+420 604 954 118";
        exhibition.Email = "brnokocky@gmail.com";
        exhibition.Iban = "CZ1320100000002600780927";
        exhibition.BankAccount = "2600780927/2010";
        exhibition.IsPublished = published;
        exhibition.OrganizationId = organizationId;

        exhibition.Address = new Address
        {
            Latitude = "49.22751891768219",
            Longitude = "16.593051747937647",
            StreetAddress = "Palackého tř. 832/126, 612 00 Brno-Královo Pole"
        };
        context.Exhibitions.Add(exhibition);

        await context.SaveChangesAsync();
        ExhibitionDay day1 = new() { Date = new DateOnly(2024, 11, 9), ExhibitionId = exhibition.Id };
        ExhibitionDay day2 = new() { Date = new DateOnly(2024, 11, 10), ExhibitionId = exhibition.Id };
        context.ExhibitionDays.Add(day1);
        context.ExhibitionDays.Add(day2);
        context.Advertisements.Add(new Advertisement
        {
            Description = "Bez propagace",
            ExhibitionId = exhibition.Id,
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            IsDefault = true
        });
        await context.SaveChangesAsync();

        List<RentedCage> list = [];
        for (int i = 0; i < 200; i++)
        {
            list.Add(new RentedCage
            {
                Length = 120,
                Width = 60,
                Height = 60,
                ExhibitionDays = [day1, day2],
                RentedTypes =
                [
                    new RentedTypeEntity { RentedType = RentedType.Single },
                    new RentedTypeEntity { RentedType = RentedType.Double }
                ]
            });
        }

        context.RentedCages.AddRange(list);
        await context.SaveChangesAsync();

        List<Group> groups_1_to_12 = groups.Where(x =>
                x.GroupId is "1" or "2" or "3" or "4" or "5" or "6" or "7" or "8" or "9" or "10" or "11" or "12")
            .ToList();
        Price price1_1_to_12 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            Groups = groups_1_to_12,
            ExhibitionDays = { day1 }
        };
        Price price2_1_to_12 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1100 },
                new Amounts { Currency = Currency.Eur, Amount = 50 }
            ],
            Groups = groups_1_to_12,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_1_to_12 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 700 },
                new Amounts { Currency = Currency.Eur, Amount = 30 }
            ],
            Groups = groups_1_to_12,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_1_to_12);
        context.Prices.Add(price2_1_to_12);
        context.Prices.Add(price3_1_to_12);

        List<Group> groups_13 = groups.Where(x => x.GroupId is "13a" or "13b" or "13c")
            .ToList();
        Price price1_13 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            Groups = groups_13,
            ExhibitionDays = { day1 }
        };
        Price price2_13 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            Groups = groups_13,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_13 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 0 },
                new Amounts { Currency = Currency.Eur, Amount = 0 }
            ],
            Groups = groups_13,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_13);
        context.Prices.Add(price2_13);
        context.Prices.Add(price3_13);

        List<Group> groups_14 = groups.Where(x => x.GroupId == "14").ToList();
        Price price1_14 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 15 }
            ],
            Groups = groups_14,
            ExhibitionDays = { day1 }
        };
        Price price2_14 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 500 },
                new Amounts { Currency = Currency.Eur, Amount = 20 }
            ],
            Groups = groups_14,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_14 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 15 }
            ],
            Groups = groups_14,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_14);
        context.Prices.Add(price2_14);
        context.Prices.Add(price3_14);

        List<Group> groups_15 = groups.Where(x =>
            x.GroupId == "15").ToList();
        Price price1_15 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 15 }
            ],
            Groups = groups_15,
            ExhibitionDays = { day1 }
        };
        Price price2_15 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 500 },
                new Amounts { Currency = Currency.Eur, Amount = 20 }
            ],
            Groups = groups_15,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_15 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 15 }
            ],
            Groups = groups_15,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_15);
        context.Prices.Add(price2_15);
        context.Prices.Add(price3_15);

        List<Group> groups_16 = groups.Where(x =>
            x.GroupId == "16").ToList();
        Price price1_16 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 800 },
                new Amounts { Currency = Currency.Eur, Amount = 35 }
            ],
            Groups = groups_16,
            ExhibitionDays = { day1 }
        };
        Price price2_16 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 1400 },
                new Amounts { Currency = Currency.Eur, Amount = 55 }
            ],
            Groups = groups_16,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_16 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 800 },
                new Amounts { Currency = Currency.Eur, Amount = 35 }
            ],
            Groups = groups_16,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_16);
        context.Prices.Add(price2_16);
        context.Prices.Add(price3_16);


        List<Group> groups_17 = groups.Where(x =>
            x.GroupId == "17").ToList();
        Price price1_17 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 25 }
            ],
            Groups = groups_17,
            ExhibitionDays = { day1 }
        };
        Price price2_17 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 500 },
                new Amounts { Currency = Currency.Eur, Amount = 40 }
            ],
            Groups = groups_17,
            ExhibitionDays = { day1, day2 }
        };
        Price price3_17 = new()
        {
            Amounts =
            [
                new Amounts { Currency = Currency.Czk, Amount = 300 },
                new Amounts { Currency = Currency.Eur, Amount = 25 }
            ],
            Groups = groups_17,
            ExhibitionDays = { day2 }
        };
        context.Prices.Add(price1_17);
        context.Prices.Add(price2_17);
        context.Prices.Add(price3_17);

        const string paymentTypesString = """
                                          [
                                            {
                                              "WorkflowName": "Payment",
                                              "Rules": [
                                                {
                                                  "RuleName": "PayByBankTransfer_CZK",
                                                  "Expression": "registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                                },
                                                {
                                                  "RuleName": "PayOnlineByCard_CZK",
                                                  "Expression": "false"
                                                },
                                                {
                                                  "RuleName": "PayInPlaceByCache_CZK",
                                                  "Expression": "false"
                                                },
                                                {
                                                  "RuleName": "PayByBankTransfer_EUR",
                                                  "Expression": "false"
                                                },
                                                {
                                                  "RuleName": "PayOnlineByCard_EUR",
                                                  "Expression": "false"
                                                },
                                                {
                                                  "RuleName": "PayInPlaceByCache_EUR",
                                                  "Expression": "!registrationToExhibition.PersonRegistration.IsPartOfCsch"
                                                }
                                              ]
                                            }
                                          ]
                                          """;

        PriceTypeWorkflow paymentTypes =
            (JsonConvert.DeserializeObject<List<Workflow>>(paymentTypesString) ?? [])
            .Select(x => new PriceTypeWorkflow(x, exhibition.Id)).First();
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
        IEnumerable<PriceAdjustmentWorkflow> catRegistrationWorkflows =
            (JsonConvert.DeserializeObject<List<Workflow>>(catRegistrationWorkflow) ?? [])
            .Select(x => new PriceAdjustmentWorkflow(x, exhibition.Id));

        context.PriceAdjustmentWorkflows.AddRange(catRegistrationWorkflows);
        context.PriceTypeWorkflows.Add(paymentTypes);
        await context.SaveChangesAsync();
    }
}
