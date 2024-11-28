#region

using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Common.Exceptions;

public static class Errors
{
    private const string WrongEms = "Neplatný EMS kód - ";

    public static readonly Error DeleteLastCatRegistrationWithPaymentError = new(
        "RegistrationToExhibitionId",
        "Nemůžete smazat poslední zaregistrovanou kočku z regstrace, která už byla zaplacena");

    public static readonly Error CannotUpdateRegistrationThatHasAlreadyBeenPaidError = new(
        "RegistrationToExhibitionId",
        "Nemůžete upravit registraci kočky, jejíž registrace již byla odeslaná");

    public static readonly Error CageDoesNotBelongToExhibitionForExhibitorError = new(
        "ExhibitorsCage",
        "Klec v dané výstavě nepatří zvolenému vystavovateli");

    public static readonly Error ExhibitedCatOrLitterMustBeFilledInError =
        new("ExhibitedCat or", "Vystavovaná kočka nebo vrh musí být zaregistrovaní");

    public static readonly Error ExhibitedCatAndLitterCannotBeFilledAtTheSameTimeError = new(
        "ExhibitedCat and Litter",
        "Vystavované kočka a vrh nesmí výt vyplněni ve stejnou dobu");

    public static readonly Error CatDaysAreNotUniqueError = new("CatDays", "Dny nejsou unikátní");

    public static readonly Error ThereAremMoreCatDaysInExhibitionError =
        new("CatDays", "Registrujete se na více dnů než kolik dnů má výstava");

    public static readonly Error CatDayDoesNotBelongToExhibitionError =
        new("CatDay", "Den nepatří do zvolené výstavy");

    public static readonly Error CatDayHasNeitherCageNorRentedCageTypeError =
        new("CatDays", "Den není registrován s půjčenou ani s vlastní klecí");

    public static readonly Error CatDayHasBothCageAndRentedCageTypeError =
        new("CatDays", "Den je registrován jak s půjčenou, tak s vlastní klecí");

    public static readonly Error YouCannotDeleteRegistrationThatHasAlreadyBeenPaidError =
        new("RegistrationToExhibitionId", "Nelze odstravit registraci, která už byla zaplacena");

    public static readonly Error CatDayHasNoGroupsError = new("GroupsIds", "Den nemá žádné skupiny");

    public static readonly Error CatDayHasGroupsThatAreNotAvailable =
        new("CatDays", "Den nemá žádné dostupné skupint");

    public static readonly Error CatIsRegisteredIntoDAysThatAreNotInExhibitionError =
        new("CatDays", "Registrace kočky do dnů, které nejsou v dané výstavě");

    public static readonly Error NonHomeCatMustHaveParents =
        new("ExhibitedCat", "Vámi zadaný typ kočky musí mít rodiče");

    public static readonly Error FatherAndMotherCannotHaveSamePedigreeNumberError = new(
        "PedigreeNumber",
        "Otec a matka nemůžou mít stejné číslo rodokmenu");

    public static readonly Error CatCannotHaveTheSamePedigreeNumberAdFatherOrMotherError = new(
        "PedigreeNumber",
        "Kočka nemůže mít stejné číslo rodokmentu jako její otec nebo matka");

    public static readonly Error HomeCatMustNotHaveParentsError =
        new("ExhibitedCat", "Vámi zvolený typ kočky nesmí mít rodiče");

    public static readonly Error ThereAreZeroExhibitedDaysError =
        new("ExhibitionDaysId", "Není zvolen žádný den");

    public static readonly Error ExhibitionDasAreNotConsistentWithExhibitionError =
        new("ExhibitionDaysId", "Dny nejsou dostupné pro danou výstavu");

    public static readonly Error DoubleCageIsTooSmallError = new("CageType", "Dvojitá klec je moc malá");

    public static readonly Error SingleCageIsTooSmallError = new("CageType", "Jednoduchá klec je moc malá");

    public static readonly Error CageNotFoundError = new("CageType", "Klec nebyla nalezena");

    public static readonly Error UnknownError = new("", "Nestala blíže nespecifikovaná chyba");

    public static readonly Func<string, Error> ErrorWithCustomMessage = message => new Error("", message);

    public static readonly Error ExhibitionIsCancelledError = new("ExhibitionId", "Výstava je zrušená");

    public static readonly Error ExhibitionIsNotPublishedError = new("ExhibitionId", "Výstava není publikovaná");

    public static readonly Error ExhibitorAlreadyRegisteredError =
        new("ExhibitorId", "Vystavovatel je už registrovaný");

    public static readonly Error RegistrationIsCloseError = new("ExhibitionId", "Registrace je uzařena");

    public static readonly Error RegistrationHasNotStartedError = new("ExhibitionId", "Registrace ještě nezačala");

    public static readonly Error PaymentIsNotCompletedYetError = new("Payment", "Platba není dokočená");

    public static readonly Error UserIsAlreadyExhibitorError = new("UserId", "Uživatel je už vystavovatel");

    public static readonly Error PaymentInfoAlreadyExistsError = new("PaymentInfo", "Registrace už byla odeslaná");

    public static readonly Error NoCatsRegisteredError = new("CatRegistrations", "Nebyly registrovány žádné kočky");

    public static readonly Error PaymentInfoNotFoundError = new("PaymentInfo", "Registrace nebyla odeslaná");

    public static readonly Error PaymentAlreadyCompletedError = new("PaymentInfo", "Platba už byla dokončena");

    public static readonly Error PaymentNotCompletedYetError = new("PaymentInfo", "Platba ještě nebyla dokočená");

    public static readonly Error CannotDeletePersonalDataWhileInActiveRegistrationError =
        new("UserId", "Není možné odstranit osobní údaje během aktivní registrace");

    public static readonly Error CannotDeletePersonalDataIfYouAreOrganiationAdminError =
        new("UserId", "Není možné odstranit osobní údaje pokud jste správce organizace");

    public static readonly Error CannotDeletePersonalDataIfYouAreAdminError =
        new("UserId", "Není možné odstranit osobní údaje pokud jste admin");

    public static readonly Error NoDefaultAdvertisementsError =
        new("AdvertisementId", "Nejsou dostupné defaultní publikace vystavovatelů");

    public static readonly Error UserAlreadyHasOrganizationError = new("UserId", "Uživatel už je správcem organizace");

    public static readonly Error OrganizationAlreadyConfirmedError =
        new("OrganizationId", "Organizace již byla potvrzena");

    public static readonly Error CannotDeleteConfirmedOrganizationError =
        new("OrganizationId", "Není možné odstranit potvrzenou organizaci");

    public static readonly Error AdvertisementWithThisDescriptionAlreadyExistsError =
        new("AdvertisementDescription", "Publikace vysatvovatelů s daným popisem již existuje");

    public static readonly Error DefaultAdvertisementAlreadyExistsError =
        new("AdvertisementId", "Defaultní publikace vystavovatelů už existuje");

    public static readonly Error PricesNotFoundError = new("Prices", "Ceny nenalazeny");

    public static readonly Error PricesAreFromDifferentExhibitionsError =
        new("Prices", "Ceny nejsou dostupné v dané výstavě");

    public static readonly Error ExhibitionDaysAreNotFromSameExhibitionError =
        new("ExhibitionDays", "Dny nejsou z jedné výstavy");

    public static readonly Error PriceAlreadyExistsError = new("Price", "Cena už existuje");

    public static readonly Error InvalidEmsCodeKodSrstioError = new(WrongEms + "kód srsti", "EMSCode");

    public static readonly Error InvalidEmsCodeZbaveniuOciError = new(WrongEms + "zbarvení očí", "EMSCode");

    public static readonly Error InvalidEmsCodeZkraceníOcasuError = new(WrongEms + "zkrácení ocasu", "EMSCode");

    public static readonly Error InvalidEmsCodeZSnizenaPigmentaceError =
        new(WrongEms + "snížená pigmentace", "EMSCode");

    public static readonly Error InvalidEmsCodeTypKresbyError = new("EMSCode", WrongEms + "typ kresby");

    public static readonly Error InvalidEmsCodeStypenDepigmentaceError =
        new("EMSCode", WrongEms + "stupen depigmentace");

    public static readonly Error InvalidEmsCodeBilaSkvrnitostError = new("EMSCode", WrongEms + "bílá skvrnitost");

    public static readonly Error InvalidEmsCodeZbarveniSrstiError = new("EMSCode", WrongEms + "zbarveni srsti");

    public static readonly Error InvalidEmsCodePlemenoError = new("EMSCode", WrongEms + "plemeno");

    public static readonly Error WrongEmsCodeError = new("EMSCode", "Neplatný EMS kod");

    public static readonly Error EmptyEmsKodError = new("EMSCode", "Ems kod nemůže být prazdný");

    public static readonly Error TrailingSpacesEmsCodeError =
        new("EMSCode", "EmsKod nesmí obsahovat mezery na začátku a na konci");

    public static readonly Error NoDaysError = new("Error.NoDays", "Žádné dny pro zvolenou výstavu nejsou dostupné.");

    public static readonly Error NoPricingError = new("Error.NoPricing",
        "Nebyl vytvořen ceník.");

    public static readonly Error IsAlreadyPublishedAdvertisementError =
        new("Error.IsPublished", "Výstava už byla publikována.");

    public static readonly Error CannotDeletePublishedExhibitionError = new("Error.CannotDeletePublishedExhibition",
        "Není možné odstranit již publikovanou výstavu.");

    public static readonly Error CannotCancelUnpublishedExhibitionError = new("Error.CannotCancelUnpublishedExhibition",
        "Není možné zrušit ještě nezveřejněnou výstavu.");

    public static readonly Error CannotUpdateCancelledExhibitionError = new("Error.CannotUpdateCancelledExhibition",
        "Není možné aktualizovat už zrušenou výstavu.");

    public static readonly Error OrganizationNotConfirmedError =
        new("Error.OrganizationNotConfirmed", "Organizace ještě není potvrzena.");

    public static readonly Error CatMustBeOlderThan4MonthsError = new("ExhibitedCat", "Kočka musí být starší 4 měsíců");

    public static readonly Error LitterMustBeOlderThan4MonthsError = new("Litter", "Vrh musí být starší 4 měsíců");

    public static readonly Error LitterMustBeYoungerThan10MonthsError = new("Litter", "Vrh musí být mladší 10 měsíců");

    public static readonly Error IfIsPartOfCschIsAutomaticallyPartFifeError =
        new("IsPartOfFife", "Každý člen ČSCH je automaticky členem FIFE");

    public static readonly Error InvalidEmsCodeError = new("EMSCode", "Neplatný EMS kód");
}
