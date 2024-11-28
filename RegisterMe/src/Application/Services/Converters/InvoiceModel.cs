#region

using RegisterMe.Domain.Enums;

#endregion

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RegisterMe.Application.Services.Converters;

public record InvoiceModel
{
    public required string Url { get; init; }

    public required string? PassOfOrigin { get; init; }
    public required string? NameOfBreedingStation { get; init; }

    public required int Id { get; init; }
    public required int NOfCatsAndLitters { get; init; }
    public required int TotalListsForCat { get; init; }
    public required int ActualList { get; init; }
    public required Currency? Currency { get; init; }
    public required decimal AmountPaid { get; init; }

    public required DateTimeOffset? DateSend { get; init; }
    public required decimal? CatPrice { get; init; }
    public required decimal? RegistrationPrice { get; init; }
    public required DateTimeOffset? DatePaymentAccepted { get; init; }
    public required PaymentType? PaymentType { get; init; }
    public required decimal? ExhibitorPrice { get; init; }
    public required string ExhibitionName { get; init; }
    public required string OrganizationName { get; init; }
    public required List<DateOnly> VisitedDays { get; init; }
    public required string? Advertisement { get; init; }
    public required bool IsCsch { get; init; }
    public bool CschY => IsCsch;
    public bool CschN => !IsCsch;

    public required string CatName { get; init; }
    public required string? CatEms { get; init; }
    public required int? CatGroup { get; init; }
    public required string? CatBreed { get; init; }
    public required string? CatColour { get; init; }
    public required string? CatPedigreeNumber { get; init; }
    public required DateOnly? CatBorn { get; init; }
    public required Gender? CatGender { get; init; }
    public bool? GenderM => CatGender == null ? null : CatGender == Gender.Male;
    public bool? GenderF => CatGender == null ? null : CatGender == Gender.Female;
    public required bool IsCastrated { get; init; }
    public bool CastratedY => IsCastrated;
    public bool CastratedN => !IsCastrated;
    public required string? BreederFirstName { get; init; }
    public required string? BreederLastName { get; init; }
    public required string? BreederCountryName { get; init; }
    public required bool BreederSameAsExhibitor { get; init; }
    public bool BreederSameAsExhibitorY => BreederSameAsExhibitor;
    public bool BreederSameAsExhibitorN => !BreederSameAsExhibitor;

    public required bool IsLitter { get; init; }
    public bool? LitterY => IsLitter;
    public bool? LitterN => !IsLitter;

    public required string? FatherName { get; init; }
    public required string? FatherEms { get; init; }
    public required string? FatherColour { get; init; }
    public required string? FatherPedigreeNumber { get; init; }

    public required string? MotherName { get; init; }
    public required string? MotherEms { get; init; }
    public required string? MotherColour { get; init; }
    public required string? MotherPedigreeNumber { get; init; }

    public required string ExhibitorSurname { get; init; }
    public required string ExhibitorFirstname { get; init; }
    public required string ExhibitorEmail { get; init; }
    public required string ExhibitorPhoneNumber { get; init; }
    public required string ExhibitorStreet { get; init; }
    public required string ExhibitorHouse { get; init; }
    public required string ExhibitorZip { get; init; }
    public required string ExhibitorCountry { get; init; }
    public required string ExhibitorCity { get; init; }
    public required DateOnly ExhibitorDateOfBirth { get; init; }
    public required string EOrganizationName { get; init; }
    public required string EMemberNumber { get; init; }

    public required string? Note { get; init; }

    public required bool One { get; init; }
    public required bool Two { get; init; }
    public required bool Three { get; init; }
    public required bool Four { get; init; }
    public required bool Five { get; init; }
    public required bool Six { get; init; }
    public required bool Seven { get; init; }
    public required bool Eight { get; init; }
    public required bool Nine { get; init; }
    public required bool Ten { get; init; }
    public required bool Eleven { get; init; }
    public required bool Twelve { get; init; }
    public required bool ThirteenA { get; init; }
    public required bool ThirteenB { get; init; }
    public required bool ThirteenC { get; init; }
    public required bool Fourteen { get; init; }
    public required bool Fifteen { get; init; }
    public required bool Sixteen { get; init; }
    public required bool Seventeen { get; init; }

    public required string? CatFeeOne { get; init; }
    public required string? CatFeeTwo { get; init; }
    public required string? CatFeeThree { get; init; }
    public required string? CatFeeFour { get; init; }
    public required string? CatFeeFive { get; init; }
    public required string? CatFeeSix { get; init; }
    public required decimal? CatPriceOne { get; init; }
    public required decimal? CatPriceTwo { get; init; }
    public required decimal? CatPriceThree { get; init; }
    public required decimal? CatPriceFour { get; init; }
    public required decimal? CatPriceFive { get; init; }
    public required decimal? CatPriceSix { get; init; }

    public required bool? IsRentedCage { get; init; }
    public bool? CageRentedY => IsRentedCage;
    public bool? CageRentedN => !IsRentedCage;
    public required bool? IsDoubleCage { get; init; }
    public bool? DoubleCageY => IsDoubleCage;
    public bool? DoubleCageN => !IsDoubleCage;
    public required int? CageLength { get; init; }
    public required int? CageWidth { get; init; }
    public required int? CageHeight { get; init; }
    public required DateTimeOffset ReportGenerated { get; init; }

    public required string? EmailToOrganization { get; init; }
}
