#region

using Microsoft.AspNetCore.Identity;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Mappings;
using RegisterMe.Application.Common.Models;
using RegisterMe.Application.Organizations.Dtos;
using RegisterMe.Application.Organizations.Enums;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Constants;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Organizations;

public class OrganizationService(
    IApplicationDbContext context,
    IMapper mapper,
    UserManager<ApplicationUser> userManager) : IOrganizationService
{
    public async Task<Result<int>> CreateOrganization(CreateOrganizationDto model,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(model.AdminId);
        Guard.Against.NotFound(model.AdminId, user);
        if (user.OrganizationId != null)
        {
            return Result.Failure<int>(Errors.UserAlreadyHasOrganizationError);
        }

        Organization organization = mapper.Map<Organization>(model);

        Guard.Against.NotFound(model.AdminId, user);
        organization.Administrator = [user];
        context.Organizations.Add(organization);
        await userManager.AddToRoleAsync(user, Roles.OrganizationAdministrator);

        await context.SaveChangesAsync(cancellationToken);


        return Result.Success(organization.Id);
    }

    public async Task<Result> UpdateOrganization(UpdateOrganizationDto model,
        CancellationToken cancellationToken = default)
    {
        Organization? organization = await context.Organizations.FindAsync([model.Id], cancellationToken);
        Guard.Against.NotFound(model.Id, organization);

        organization.Email = model.Email;
        organization.Name = model.Name;
        organization.TelNumber = model.TelNumber;
        organization.Website = model.Website;
        organization.Address = model.Address;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<PaginatedList<OrganizationDto>> GetOrganizations(int pageNumber, int pageSize,
        OrganizationConfirmationStatus isConfirmed,
        GetOrganizationsModel? parameters = null)
    {
        IQueryable<Organization> organizationQuery = context.Organizations.Include(x => x.Exhibitions).AsQueryable();

        if (parameters != null)
        {
            organizationQuery = ApplyFilters(parameters, organizationQuery);
        }

        if (isConfirmed != OrganizationConfirmationStatus.All)
        {
            bool isConfirmedBool = isConfirmed == OrganizationConfirmationStatus.Confirmed;
            organizationQuery = organizationQuery.Where(x => x.IsConfirmed == isConfirmedBool);
        }

        PaginatedList<Organization> exhibitions = await organizationQuery
            .Include(x => x.Exhibitions)
            .OrderBy(x => x.Id)
            .PaginatedListAsync(pageNumber, pageSize);

        List<OrganizationDto> models = [];
        foreach (Organization? model in exhibitions.Items)
        {
            IEnumerable<int> publishedExhibitionsIds = model.Exhibitions.Where(x => x.IsPublished).Select(x => x.Id);
            OrganizationDto? exhibitionDto = mapper.Map<OrganizationDto>(model);
            exhibitionDto.ExhibitionIds = publishedExhibitionsIds.ToList();
            models.Add(exhibitionDto);
        }

        PaginatedList<OrganizationDto> paginatedListVm = new(models,
            exhibitions.TotalCount, pageNumber, pageSize);

        return paginatedListVm;
    }

    public async Task<OrganizationDto> GetOrganizationByIdAsync(int organizationId,
        CancellationToken cancellationToken = default)
    {
        Organization? organization = await context.Organizations
            .Where(x => x.Id == organizationId)
            .Include(x => x.Exhibitions)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(organizationId, organization);
        return mapper.Map<OrganizationDto>(organization);
    }

    public async Task<Result> ConfirmOrganization(int organizationId,
        CancellationToken cancellationToken = default)
    {
        Organization? organization = await context.Organizations.FindAsync([organizationId], cancellationToken);
        Guard.Against.NotFound(organizationId, organization);
        if (organization.IsConfirmed)
        {
            return Result.Failure(Errors.OrganizationAlreadyConfirmedError);
        }

        organization.IsConfirmed = true;
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteOrganization(int organizationId,
        CancellationToken cancellationToken = default)
    {
        Organization organization = await context.Organizations.Where(x => x.Id == organizationId)
            .Include(x => x.Administrator)
            .SingleAsync(cancellationToken);
        List<ApplicationUser> administrators = organization.Administrator;
        if (organization.IsConfirmed)
        {
            return Result.Failure(Errors.CannotDeleteConfirmedOrganizationError);
        }

        organization.Administrator = [];
        context.Organizations.Update(organization);
        await context.SaveChangesAsync(cancellationToken);
        context.Organizations.Remove(organization);

        foreach (ApplicationUser? administrator in administrators)
        {
            await userManager.RemoveFromRoleAsync(administrator, Roles.OrganizationAdministrator);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<bool> IsOrganizationAdministrator(string userId, int organizationId)
    {
        return await context.Organizations
            .Where(x => x.Id == organizationId)
            .AnyAsync(x => x.Administrator.Select(y => y.Id).Contains(userId));
    }

    private IQueryable<Organization> ApplyFilters(GetOrganizationsModel parameters,
        IQueryable<Organization> query)
    {
        if (!string.IsNullOrEmpty(parameters.SearchString))
        {
            string searchString = parameters.SearchString.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(searchString)
                                     || s.Email.ToLower().Contains(searchString)
                                     || s.Ico.ToLower().Contains(searchString)
                                     || s.TelNumber.ToLower().Contains(searchString)
                                     || s.Website.ToLower().Contains(searchString)
                                     || s.Address.ToLower().Contains(searchString));
        }

        if (parameters.HasExhibitions != null)
        {
            query = parameters.HasExhibitions switch
            {
                HasExhibitions.Yes => query.Where(x => x.Exhibitions.Any()),
                HasExhibitions.No => query.Where(x => !x.Exhibitions.Any()),
                HasExhibitions.YesConfirmedOnly => query.Where(x =>
                    x.Exhibitions.Any(exhibition => exhibition.IsPublished)),
                HasExhibitions.YesNotConfirmedOnly => query.Where(x =>
                    x.Exhibitions.Any(exhibition => !exhibition.IsPublished)),
                _ => query
            };
        }

        return query;
    }
}
