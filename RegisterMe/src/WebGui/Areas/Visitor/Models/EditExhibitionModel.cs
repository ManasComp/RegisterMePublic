#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Domain.Entities.RulesEngine;
using RulesEngine.Models;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class EditExhibitionModel
{
    public required BriefExhibitionDto Exhibition { get; init; } = null!;
    public required List<AdvertisementDto> Advertisements { get; init; } = null!;
    public required Workflow? PaymentTypes { get; init; }
    public required List<WorkflowDto> Discounts { get; init; } = [];
    public required List<BigPriceDto> Prices { get; init; } = [];
    public required List<BriefCageDto> Cages { get; init; } = [];
    public required bool CanPayByCard { get; init; }
    public required bool IsFullyRegistered { get; init; }
}
