#region

using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.ValueTypes;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public static class AdvertisementDataGenerator
{
    public static UpsertAdvertisementDto GetAdvertisementDto1()
    {
        UpsertAdvertisementDto advertisement = new()
        {
            IsDefault = true, Description = "Advertisement Description", Price = new MultiCurrencyPrice(100, 8)
        };

        return advertisement;
    }

    public static UpsertAdvertisementDto GetAdvertisementDto2()
    {
        UpsertAdvertisementDto advertisement = new()
        {
            IsDefault = true, Description = "Advertisement2 Description", Price = new MultiCurrencyPrice(200, 10)
        };

        return advertisement;
    }
}
