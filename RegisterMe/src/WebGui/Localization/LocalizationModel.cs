#region

using XLocalizer.ErrorMessages;

#endregion

namespace WebGui.Localization;

public record LocalizationModel
{
    public required ValidationErrors ValidationErrors { get; init; }
    public required ModelBindingErrors ModelBindingErrors { get; init; }
    public required IdentityErrors IdentityErrors { get; init; }
}
