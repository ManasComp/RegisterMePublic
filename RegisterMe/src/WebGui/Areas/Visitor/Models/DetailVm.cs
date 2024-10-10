#region

using RegisterMe.Application.Pricing.Dtos;
using RegisterMe.Domain.Enums;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class DetailVm
{
    public required RegistrationToExhibitionPrice RegistrationToExhibitionPrice { get; init; } = null!;
    public required string ExhibitionName { get; init; } = null!;
    public required int? ExhibitionId { get; init; }
    public required int RegistrationToExhibitionId { get; init; }
    public required DateTimeOffset PaymentRequestDate { get; init; }
    public required DateTimeOffset? PaymentCompletedDate { get; init; }
    public required PaymentType PaymentType { get; init; }
    public required bool IsOrganizationAdministrator { get; init; }
    public required List<Currency> CurrencyUserCanPayIn { get; init; } = null!;
    public required Currency Currency { get; init; }
    public required decimal? AmountPaid { get; init; }
    public required bool EmsOk { get; init; }
}
