#region

using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Exhibitors;

public class ExhibitorService(IApplicationDbContext appContext, IMapper mapper) : IExhibitorService
{
    public async Task<Result<int>> CreateExhibitor(UpsertExhibitorDto exhibitorDto, string userId,
        CancellationToken cancellationToken = default)
    {
        if (IsExhibitor(userId))
        {
            return Result.Failure<int>(Errors.UserIsAlreadyExhibitorError);
        }

        if (exhibitorDto is { IsPartOfCsch: true, IsPartOfFife: false })
        {
            return Result.Failure<int>(Errors.IfIsPartOfCschIsAutomaticallyPartFifeError);
        }

        Exhibitor exhibitor = new()
        {
            AspNetUserId = userId,
            Organization = exhibitorDto.Organization,
            MemberNumber = exhibitorDto.MemberNumber,
            Country = exhibitorDto.Country,
            City = exhibitorDto.City,
            Street = exhibitorDto.Street,
            HouseNumber = exhibitorDto.HouseNumber,
            ZipCode = exhibitorDto.ZipCode,
            IsPartOfCsch = exhibitorDto.IsPartOfCsch,
            EmailToOrganization = exhibitorDto.EmailToOrganization,
            IsPartOfFife = exhibitorDto.IsPartOfFife
        };

        appContext.Exhibitors.Add(exhibitor);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success(exhibitor.Id);
    }

    public async Task<ExhibitorAndUserDto?> GetExhibitorByUserId(string userId,
        CancellationToken cancellationToken = default)
    {
        Exhibitor? exhibitor = await appContext.Exhibitors
            .Where(x => x.AspNetUserId == userId)
            .Include(x => x.AspNetUser)
            .SingleOrDefaultAsync(cancellationToken);

        if (exhibitor == null)
        {
            return null;
        }

        ExhibitorAndUserDto exhibitorAndUserDto = mapper.Map<ExhibitorAndUserDto>(exhibitor);

        return exhibitorAndUserDto;
    }

    public async Task<ExhibitorAndUserDto> GetExhibitorById(int exhibitorId,
        CancellationToken cancellationToken = default)
    {
        Exhibitor? exhibitor = await appContext.Exhibitors
            .Where(x => x.Id == exhibitorId)
            .Include(x => x.AspNetUser)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(exhibitorId, exhibitor);

        ExhibitorAndUserDto exhibitorAndUserDto = mapper.Map<ExhibitorAndUserDto>(exhibitor);

        return exhibitorAndUserDto;
    }

    public async Task<ExhibitorAndUserDto> GetExhibitorByRegistrationToExhibitionId(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Include(x => x.Exhibitor)
            .ThenInclude(x => x.AspNetUser)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionId, registrationToExhibition);

        ExhibitorAndUserDto exhibitorAndUserDto = mapper.Map<ExhibitorAndUserDto>(registrationToExhibition.Exhibitor);

        return exhibitorAndUserDto;
    }

    public async Task<Result> UpdateExhibitor(UpsertExhibitorDto exhibitorDto, string userId,
        CancellationToken cancellationToken = default)
    {
        Exhibitor exhibitor = await appContext.Exhibitors
            .Where(x => x.AspNetUserId == userId)
            .Include(x => x.AspNetUser)
            .SingleAsync(cancellationToken);


        if (exhibitorDto is { IsPartOfCsch: true, IsPartOfFife: false })
        {
            return Result.Failure(Errors.IfIsPartOfCschIsAutomaticallyPartFifeError);
        }

        exhibitor.Organization = exhibitorDto.Organization;
        exhibitor.MemberNumber = exhibitorDto.MemberNumber;
        exhibitor.Country = exhibitorDto.Country;
        exhibitor.City = exhibitorDto.City;
        exhibitor.Street = exhibitorDto.Street;
        exhibitor.HouseNumber = exhibitorDto.HouseNumber;
        exhibitor.ZipCode = exhibitorDto.ZipCode;
        exhibitor.IsPartOfCsch = exhibitorDto.IsPartOfCsch;
        exhibitor.EmailToOrganization = exhibitorDto.EmailToOrganization;
        exhibitor.IsPartOfFife = exhibitorDto.IsPartOfFife;

        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private bool IsExhibitor(string usedId)
    {
        return appContext.Exhibitors.Any(x => x.AspNetUserId == usedId);
    }
}
