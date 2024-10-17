#region

using RegisterMe.Application.Services.Enums;

#endregion

namespace RegisterMe.Application.Pricing.Dtos;

public record PricingCage
{
    /// <summary>
    ///     Number of days the cage is rented
    /// </summary>
    public int NumberOfDays { get; set; }

    /// <summary>
    ///     Length of the cage
    /// </summary>
    public int Length { get; init; }

    /// <summary>
    ///     Width of the cage
    /// </summary>
    public int Width { get; init; }

    /// <summary>
    ///     Height of the cage
    /// </summary>
    public int Height { get; init; }

    /// <summary>
    ///     Type of the cage
    /// </summary>
    public OwnCageEnum Type { get; init; }
}
