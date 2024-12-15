#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Application.Pricing.Enums;
using RegisterMe.Domain.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class PayInformationVm
{
    public required RegistrationToExhibitionPrice RegistrationToExhibitionPrice { get; init; } = null!;
    public required string ExhibitionName { get; init; } = null!;
    public required int? ExhibitionId { get; init; }
    public required string Message { get; init; } = null!;
    public required int RegistrationToExhibitionId { get; init; }
    public required List<CatRegistrationStatistics> CatRegistrationStats { get; init; } = null!;
    public required Dictionary<Currency, string> QrCodes { get; init; } = null!;
    public required string Iban { get; init; } = null!;
    public required string NormalAccount { get; init; } = null!;
    public required int VariableSymbol { get; init; }
    public required Dictionary<Currency, bool> CanPay { get; init; }
    public required List<PaymentTypeWithCurrency> PaymentTypes { get; init; } = null!;
    public required List<Currency> CurrencyUserCanPayIn { get; init; } = null!;
}
