#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public record RegistrationToExhibitionDto : CreateRegistrationToExhibitionDto
{
    /// <summary>
    ///     Payment info
    /// </summary>
    public required PaymentInfoDto? PaymentInfo { get; init; }

    /// <summary>
    ///     Person registration
    /// </summary>
    public required PersonRegistrationDto PersonRegistration { get; init; } = null!;

    /// <summary>
    ///     Registration id
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    ///     Cat registration ids
    /// </summary>
    public required List<int> CatRegistrationIds { get; init; }

    /// <summary>
    ///     Get order status
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public OrderStatus OrderStatus()
    {
        if (PaymentInfo != null)
        {
            return PaymentInfo.PaymentCompletedDate == null
                ? Domain.Enums.OrderStatus.PaymentInProgress
                : Domain.Enums.OrderStatus.PaymentCompleted;
        }

        return CatRegistrationIds.Count switch
        {
            > 0 => Domain.Enums.OrderStatus.RegisteredWithCats,
            0 => Domain.Enums.OrderStatus.RegisteredWithoutCats,
            _ => throw new InvalidOperationException("OrderStatus not found")
        };
    }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.RegistrationToExhibition, RegistrationToExhibitionDto>()
                .ForMember(x => x.PaymentInfo, opt => opt.MapFrom(src => src.PaymentInfo))
                .ForMember(x => x.CatRegistrationIds,
                    opt => opt.MapFrom(src => src.CatRegistrations.Select(x => x.Id)));
        }
    }
}
