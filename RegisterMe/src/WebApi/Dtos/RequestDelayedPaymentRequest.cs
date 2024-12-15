#region

using RegisterMe.Domain.Enums;

#endregion

namespace WebApi.Dtos;

public record RequestDelayedPaymenParamRequest
{
    public required PaymentType PaymentType { get; init; }
    public required Currency Currency { get; init; }
}
