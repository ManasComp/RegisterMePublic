#region

using System.Collections.Immutable;
using Newtonsoft.Json;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exceptions;
using RegisterMe.Application.RegistrationToExhibition;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Services.Converters;

public class JsonExporterService(IApplicationDbContext applicationDbContext) : IJsonExporterService
{
    public async Task<string> GetDataAsync(int exhibitionId)
    {
        // https://github.com/dotnet/efcore/issues/17212
        List<Domain.Entities.RegistrationToExhibition> registrations = await applicationDbContext
            .RegistrationsToExhibition
            .Where(x => x.ExhibitionId == exhibitionId)
            .Include(x => x.PaymentInfo)
            .Where(RegistrationToExhibitionService.WasPaid)
            .Include(x => x.Exhibitor)
            .ThenInclude(x => x.AspNetUser)
            .Include(x => x.CatRegistrations)
            .ThenInclude(x => x.CatDays)
            .ThenInclude(x => x.Groups)

            //exhibitedCat
            .Include(x => x.CatRegistrations)
            .ThenInclude(x => x.ExhibitedCat)
            .ThenInclude(x => x!.Mother)
            .Include(x => x.CatRegistrations)
            .ThenInclude(x => x.ExhibitedCat)
            .ThenInclude(x => x!.Father)
            .Include(x => x.CatRegistrations)
            .ThenInclude(x => x.ExhibitedCat)
            .ThenInclude(x => x!.Breeder)

            //litter
            .Include(x => x.CatRegistrations)
            .ThenInclude(x => x.Litter)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();

        List<ExportPerson> results = registrations.Select(CreateExportPerson).ToList();
        string json = JsonConvert.SerializeObject(results);
        return json;
    }

    private ExportPerson CreateExportPerson(Domain.Entities.RegistrationToExhibition registration)
    {
        return new ExportPerson
        {
            Name = $"{registration.Exhibitor.AspNetUser.FirstName} {registration.Exhibitor.AspNetUser.LastName}",
            Country = registration.Exhibitor.Country,
            MemberOf = registration.Exhibitor.Organization,
            Cats = registration.CatRegistrations.Select(CreateExportCat).Where(x => x != null).Select(x => x!)
                .ToList()
        };
    }

    private ExportCat? CreateExportCat(CatRegistration catRegistration)
    {
        ExhibitedCat? exhibitedCat = catRegistration.ExhibitedCat;
        Litter? litter = catRegistration.Litter;

        if (litter != null)
        {
            return null;
        }

        if (exhibitedCat == null)
        {
            throw new InvalidDatabaseStateException();
        }

        Result<EmsCode> ems = EmsCode.Create(exhibitedCat.Ems);
        if (ems.IsFailure)
        {
            throw new InvalidDatabaseStateException();
        }

        ParsedEms parsedEms = ems.Value.GetEms();
        return new ExportCat
        {
            Name = $"{exhibitedCat.TitleBeforeName} {exhibitedCat.Name} {exhibitedCat.TitleAfterName}".Trim(),
            Sex = exhibitedCat.Sex.ToString(),
            PedigreeNumber = exhibitedCat.PedigreeNumber,
            Group = exhibitedCat.Group?.ToString(),
            Class = string.Join(",",
                catRegistration.CatDays.SelectMany(cd => cd.Groups).Select(g => g.GroupId).ToImmutableSortedSet()),
            DateOfBirth = exhibitedCat.BirthDate.ToString("yyyy-MM-dd"),
            Breeder =
                exhibitedCat.Breeder != null
                    ? $"{exhibitedCat.Breeder.FirstName} {exhibitedCat.Breeder.LastName}"
                    : null,
            Breed = parsedEms.Breed,
            Variant = parsedEms.Variant,
            Mother = CreateParent(exhibitedCat.Mother, exhibitedCat.Breed),
            Father = CreateParent(exhibitedCat.Father, exhibitedCat.Breed)
        };
    }

    private ExportParent? CreateParent(Parent? exhibitedCatParent, string breed)
    {
        Parent? parent = exhibitedCatParent;

        if (parent == null)
        {
            return null;
        }

        return new ExportParent
        {
            Name = $"{parent.TitleBeforeName}{parent.Name}{parent.TitleAfterName}",
            Breed = breed,
            Variant = parent.Ems ?? string.Empty,
            Group = string.Empty
        };
    }
}
