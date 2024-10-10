#region

using RegisterMe.Application.RegistrationToExhibition.Dtos;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public static class RegistrationToExhibitionDataGenerator
{
    public static CreateRegistrationToExhibitionDto Normal(int exhibitionId, int exhibitorId,
        int advertisementId)
    {
        CreateRegistrationToExhibitionDto registrationToExhibition = new()
        {
            ExhibitionId = exhibitionId, ExhibitorId = exhibitorId, AdvertisementId = advertisementId
        };

        return registrationToExhibition;
    }
}
