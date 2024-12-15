namespace RegisterMe.Domain.Enums;

/// <summary>
///     What step in the registration process is the user in
/// </summary>
public enum StepInRegistration
{
    StartedExhibitedCatAndBreederInit = 0,

    FinishedExhibitedCatAndLitterAndBreederInit = 1,
    StartedFather = 1,

    FinishedFather = 2,
    StartedMother = 2,

    FinishedMother = 3,
    StartedExhibitionSettings = 3,

    FinishedExhibitionSettings = 4,
    StartedSummary = 4,

    FinishedSummary = 5
}
