#region

using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public record PaymentInfoDto
{
    /// <summary>
    ///     Payment request date
    /// </summary>
    public required DateTimeOffset PaymentRequestDate { get; init; }

    /// <summary>
    ///     Payment completed date
    /// </summary>
    public required DateTimeOffset? PaymentCompletedDate { get; init; }

    /// <summary>
    ///     Payment intent id - used in stripe
    /// </summary>
    public required string? PaymentIntentId { get; init; }

    /// <summary>
    ///     Amount paid / to be paid
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    ///     Payment type
    /// </summary>
    public required PaymentType PaymentType { get; init; }

    /// <summary>
    ///     Currency
    /// </summary>
    public required Currency Currency { get; init; }

    /// <summary>
    ///     Session id - used in stripe
    /// </summary>
    public required string? SessionId { get; init; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PaymentInfo, PaymentInfoDto>()
                .ForMember(x => x.Currency, opt => opt.MapFrom(x => x.Amounts.Currency))
                .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Amounts.Amount));
        }
    }
}
