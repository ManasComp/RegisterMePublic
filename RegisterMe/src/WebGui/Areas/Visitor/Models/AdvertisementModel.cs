#region

using RegisterMe.Application.Exhibitions.Dtos;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class AdvertisementModel
{
    public required decimal PriceKc { get; init; }
    public required decimal PriceEur { get; init; }
    public required int ExhibitionId { get; init; }
    public required string Description { get; init; }
    public required int? Id { get; init; }
    public required bool IsDefault { get; init; }

    public static AdvertisementModel CreateBlank(int exhibitionId)
    {
        return new AdvertisementModel
        {
            PriceKc = 0,
            PriceEur = 0,
            ExhibitionId = exhibitionId,
            Description = string.Empty,
            IsDefault = false,
            Id = null
        };
    }

    public static AdvertisementModel CreateFromDto(int exhibitionId, AdvertisementDto advertisementDto)
    {
        return new AdvertisementModel
        {
            PriceKc = advertisementDto.Price.PriceCzk,
            PriceEur = advertisementDto.Price.PriceEur,
            ExhibitionId = exhibitionId,
            Description = advertisementDto.Description,
            Id = advertisementDto.Id,
            IsDefault = advertisementDto.IsDefault
        };
    }
}
