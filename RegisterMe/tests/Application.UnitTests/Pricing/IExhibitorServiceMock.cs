#region

using RegisterMe.Application.Exhibitors;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.UnitTests.Pricing;

public class ExhibitorServiceMock : IExhibitorService
{
    public Task<Result<int>> CreateExhibitor(UpsertExhibitorDto exhibitorDto, string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ExhibitorAndUserDto?> GetExhibitorByUserId(string userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ExhibitorAndUserDto> GetExhibitorById(int exhibitorId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ExhibitorAndUserDto> GetExhibitorByRegistrationToExhibitionId(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateExhibitor(UpsertExhibitorDto exhibitorDto, string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
