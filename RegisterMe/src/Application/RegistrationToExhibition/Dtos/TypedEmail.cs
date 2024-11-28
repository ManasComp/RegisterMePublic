#region

using Type = RegisterMe.Application.RegistrationToExhibition.Enums.Type;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public record TypedEmail(string Email, Type Type, int ExhibitionId, DateTimeOffset? LastNotificationSendOn);

public record SimpleTypedEmail(string Email, int ExhibitionId);
