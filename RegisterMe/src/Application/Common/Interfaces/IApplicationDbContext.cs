#region

using Microsoft.EntityFrameworkCore.Infrastructure;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;

#endregion

namespace RegisterMe.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Address> Addresses { get; }
    DbSet<CatDay> CatDays { get; }
    DbSet<CatRegistration> CatRegistrations { get; }
    DbSet<ExhibitionDay> ExhibitionDays { get; }
    DbSet<ExhibitedCat> ExhibitedCats { get; }
    DbSet<Litter> Litters { get; }
    DbSet<Exhibition> Exhibitions { get; }
    DbSet<PersonRegistration> PersonRegistrations { get; }
    DbSet<PaymentInfo> PaymentInfos { get; }
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<Domain.Entities.RegistrationToExhibition> RegistrationsToExhibition { get; }
    DbSet<Exhibitor> Exhibitors { get; }
    DbSet<RentedCage> RentedCages { get; }
    DbSet<RentedTypeEntity> RentedTypes { get; }
    DbSet<PersonCage> Cages { get; }
    DbSet<Advertisement> Advertisements { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Price> Prices { get; }
    DbSet<Group> Groups { get; }
    DbSet<PriceAdjustmentWorkflow> PriceAdjustmentWorkflows { get; }
    DbSet<PriceTypeWorkflow> PriceTypeWorkflows { get; }
    DatabaseFacade DatabaseFacade { get; }
    DbSet<Amounts> Amounts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
