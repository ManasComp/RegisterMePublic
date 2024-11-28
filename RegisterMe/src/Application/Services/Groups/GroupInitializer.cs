#region

using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Services.Groups;

#region

using static HelperMethods;

#endregion

public class GroupInitializer
{
    private const string SupremeChampion = "SC";
    private const string EuropeanChampion = "EC";
    private const string AmericanChampion = "FAC";

    private const string SupremePremior = "SP";
    private const string EuropeanPremior = "EP";
    private const string AmericanPremior = "FAP";

    private const string InternationalGrandChampion = "GIC";
    private const string InternationalGrandPremior = "GIP";

    private const string InternationalChampion = "IC";
    private const string InternationalPremior = "IP";

    private const string Champion = "CH";
    private const string Premior = "PR";

    private static List<GroupDto>? s_groupList;

    public List<GroupDto> GetGroups()
    {
        return s_groupList ??= InitializeGroups();
    }

    // https://www.cat-birma.cz/clanky/vystava-kocek/
    // https://www.schk.cz/rad_vyst.html
    private static List<GroupDto> InitializeGroups()
    {
        // Jakákoliv kočka může tedy mít pouze jednu normální třídu a nedomácí kočky mohou mít navíc i jednu libovolnou třídu 13 a veteran.

        List<GroupDto> list =
        [
            new()
            {
                Name = "EUROPA CHAMPION",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                SupremeChampion,
                                EuropeanChampion,
                                AmericanChampion))
                        .AddFilter(() => IsNotNeutered(filter.CatRegistration))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "1"
            },

            //Do této třídy náleží kastrované kočky, které již obdržely titul Supreme premior (SP), Evropský premior (EP) nebo Americký premior FIFé (FAP).

            new()
            {
                Name = "EUROPA PREMIOR",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                SupremePremior,
                                EuropeanPremior,
                                AmericanPremior))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "2"
            },

            //Do této třídy náleží kočky, které již obdržely titul Mezinárodní grandšampion (GIC).

            new()
            {
                Name = "Gr. Int. Champion (CACE)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                InternationalGrandChampion))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNotNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "3"
            },

            //Do této třídy náleží kastrované kočky, které již obdržely titul Mezinárodní grandpremior (GIP)

            new()
            {
                Name = "Gr. Int. Premior (CAPE)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() => HasTitle(filter.CatRegistration,
                            InternationalGrandPremior))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "4"
            },

            //Do této třídy náleží kočky, které již obdržely titul Mezinárodní šampion (IC).

            new()
            {
                Name = "Internationaler Champion (CAGCIB)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                InternationalChampion))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNotNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "5"
            },

            // Do této třídy náleží kastrované kočky, které již obdržely titul Mezinárodní premior (IP)

            new()
            {
                Name = "Internationaler Premior (CAGPIB)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                InternationalPremior))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "6"
            },

            // Do této třídy náleží kočky, které již obdržely titul Šampion (CH)

            new()
            {
                Name = "Champion (CACIB)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                Champion))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNotNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "7"
            },

            // Do této třídy náleží kastrované kočky, které již obdržely titul Premior (PR)

            new()
            {
                Name = "Premior (CAPIB)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            HasTitle(filter.CatRegistration,
                                Premior))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNeutered(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "8"
            },

            // Tato zvířata musí během výstavy dosáhnout věku minimálně 10 měsíců.

            new()
            {
                Name = "Otevřená (CAC)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOlderThan(FromMoths(10),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNotNeutered(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "9"
            },

            // Tato zvířata musí během výstavy dosáhnout věku minimálně 10 měsíců. 

            new()
            {
                Name = "Kastráti (CAP)",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOlderThan(FromMoths(10),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() => IsNeutered(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "10"
            },

            new()
            {
                Name = "8-12 měsíců",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOldBetween(FromMoths(8),
                                FromMoths(12),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "11"
            },

            new()
            {
                Name = "4-8 měsíců",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOldBetween(FromMoths(4),
                                FromMoths(8),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "12"
            },

            // Novic je kočka, jejíž rodiče nejsou známi, nebo kočka bez rodokmenu. Kočka může být v této třídě vystavena pouze ve vlastní zemi a ve stáří nejméně 10 měsíců, a to po kontrole národní chovatelskou komisí za dodržení Chovatelského řádu. Ve třídě noviců může být zvíře vystaveno pouze jednou. V této třídě je kočka posouzena dvěma mezinárodními posuzovateli FIFé.

            new()
            {
                Name = "Novici",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            DoesNotHaveParents(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOlderThan(FromMoths(12),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "13a"
            },

            new()
            {
                Name = "Kontrolní třída",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOlderThan(FromMoths(4),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "13b"
            },

            new()
            {
                //vlastni zeme
                Name = "Ověřovací třída",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "13c"
            },

            new()
            {
                Name = "Domácí kočky",
                Filter = filter =>
                {
                    Evaluator result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() =>
                            DoesNotHaveParents(filter.CatRegistration))
                        .AddFilter(() => filter.CatRegistration.ExhibitedCat!.PedigreeNumber == null)
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .AddFilter(() => filter.CatRegistration.ExhibitedCat!.Breeder == null);

                    if (result.Evaluate())
                    {
                        Result<EmsCode> ems = EmsCode.Create(filter.CatRegistration.ExhibitedCat!.Ems);

                        if (ems.IsFailure)
                        {
                            result.AddFilter(() => false);
                        }

                        ParsedEms parsedEms = ems.Value.GetEms();
                        result.AddFilter(() => parsedEms.Breed.Contains("HCS") || parsedEms.Breed.Contains("HCL"));
                    }

                    result.AddFilter(() => IsNeutered(filter.CatRegistration) || IsYoungerThan(FromMoths(12),
                        filter.CatRegistration.ExhibitedCat!.BirthDate, filter.ExhibitionDayDate));

                    return result.Evaluate();
                },
                GroupId = "14"
            },

            new()
            {
                Name = "Mimo soutěž",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();
                    return result;
                },
                GroupId = "15"
            },
            new()
            {
                Name = "Vrhy (min. 3)",
                Filter =
                    filter =>
                    {
                        bool result = new Evaluator()
                            .AddFilter(() => IsLitter(filter.CatRegistration))
                            .AddFilter(() => HasParents(filter.CatRegistration))
                            .AddFilter(() => IsFife(filter.CatRegistration))
                            .Evaluate();
                        return result;
                    },
                GroupId = "16"
            },

            new()
            {
                Name = "Veteran",
                Filter = filter =>
                {
                    bool result = new Evaluator()
                        .AddFilter(() => IsExhibitedCat(filter.CatRegistration))
                        .AddFilter(() => HasParents(filter.CatRegistration))
                        .AddFilter(() =>
                            IsOlderThan(FromMoths(10 * 12),
                                filter.CatRegistration.ExhibitedCat!.BirthDate,
                                filter.ExhibitionDayDate))
                        .AddFilter(() => IsFife(filter.CatRegistration))
                        .Evaluate();

                    return result;
                },
                GroupId = "17"
            }
        ];

        list.ForEach(x => x.Name = x.Name + " - " + x.GroupId);
        return list;
    }


    public class Evaluator
    {
        private readonly List<Func<bool>> _filters = [];

        public Evaluator AddFilter(Func<bool> filter)
        {
            _filters.Add(filter);
            return this;
        }

        public bool Evaluate()
        {
            return _filters.All(x => x());
        }
    }

    public record FilterParameter(
        DateOnly ExhibitionDayDate,
        LitterOrExhibitedCatDto CatRegistration);
}
