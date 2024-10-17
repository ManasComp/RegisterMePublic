#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace RegisterMe.Application.Services.Groups;

public class HelperMethods
{
    public static bool IsFife(LitterOrExhibitedCatDto registration)
    {
        return registration.ExhibitorDto.IsPartOfFife;
    }

    public static TimeSpan FromMoths(int months)
    {
        return TimeSpan.FromDays(months * 365.2425 / 12);
    }

    public static bool HasTitle(LitterOrExhibitedCatDto registration,
        params string[] titles)
    {
        if (IsLitter(registration))
        {
            throw new InvalidOperationException("Litter cannot have titles");
        }

        if (registration.ExhibitedCat?.TitleBeforeName == null && registration.ExhibitedCat?.TitleAfterName == null)
        {
            return false;
        }

        bool beforeNameContains = titles.Any(title =>
            registration.ExhibitedCat.TitleBeforeName != null &&
            registration.ExhibitedCat.TitleBeforeName.Contains(
                title.ToLower(),
                StringComparison.CurrentCultureIgnoreCase));

        bool afterNameContains = titles.Any(title =>
            registration.ExhibitedCat.TitleAfterName != null &&
            registration.ExhibitedCat.TitleAfterName.Contains(
                title.ToLower(),
                StringComparison.CurrentCultureIgnoreCase));

        return beforeNameContains || afterNameContains;
    }

    public static bool IsExhibitedCat(LitterOrExhibitedCatDto registration)
    {
        return registration is
        {
            ExhibitedCat: not null,
            LitterDto: null
        };
    }

    public static bool IsLitter(LitterOrExhibitedCatDto registration)
    {
        return registration is
        {
            ExhibitedCat: null,
            LitterDto: not null
        };
    }


    public static bool HasParents(LitterOrExhibitedCatDto registration)
    {
        if (IsLitter(registration))
        {
            return true;
        }

        if (IsExhibitedCat(registration))
        {
            return registration.ExhibitedCat is { Mother: not null, Father: not null };
        }

        throw new InvalidOperationException("Registration is not litter or exhibited cat");
    }

    public static bool DoesNotHaveParents(LitterOrExhibitedCatDto registration)
    {
        return !HasParents(registration);
    }

    public static bool IsOldBetween(TimeSpan from,
        TimeSpan to,
        DateOnly birthDate,
        DateOnly exhibitionDay)
    {
        return IsOlderThan(from,
                   birthDate,
                   exhibitionDay) &&
               IsYoungerThan(to,
                   birthDate,
                   exhibitionDay);
    }

    public static bool IsOlderThan(TimeSpan from,
        DateOnly birthDate,
        DateOnly exhibitionDay)
    {
        return exhibitionDay.ToDateTime(new TimeOnly()) - birthDate.ToDateTime(new TimeOnly()) > from;
    }

    public static bool IsYoungerThan(TimeSpan from,
        DateOnly birthDate,
        DateOnly exhibitionDay)
    {
        return exhibitionDay.ToDateTime(new TimeOnly()) - birthDate.ToDateTime(new TimeOnly()) < from;
    }

    public static bool IsNeutered(LitterOrExhibitedCatDto registration)
    {
        if (IsLitter(registration))
        {
            throw new InvalidOperationException("Litter cannot be neutered");
        }

        return registration.ExhibitedCat?.Neutered ?? false;
    }

    public static bool IsNotNeutered(LitterOrExhibitedCatDto registration)
    {
        return !IsNeutered(registration);
    }
}
