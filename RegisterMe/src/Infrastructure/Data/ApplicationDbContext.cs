#region

using System.Reflection;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;

#endregion

namespace RegisterMe.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext, IDataProtectionKeyContext
{
    public DbSet<Breeder> Breeders => Set<Breeder>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<CatDay> CatDays => Set<CatDay>();
    public DbSet<CatRegistration> CatRegistrations => Set<CatRegistration>();
    public DbSet<ExhibitionDay> ExhibitionDays => Set<ExhibitionDay>();
    public DbSet<ExhibitedCat> ExhibitedCats => Set<ExhibitedCat>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Litter> Litters => Set<Litter>();
    public DbSet<Exhibition> Exhibitions => Set<Exhibition>();
    public DbSet<PersonRegistration> PersonRegistrations => Set<PersonRegistration>();
    public DbSet<PaymentInfo> PaymentInfos => Set<PaymentInfo>();
    public DbSet<RegistrationToExhibition> RegistrationsToExhibition => Set<RegistrationToExhibition>();
    public DbSet<Exhibitor> Exhibitors => Set<Exhibitor>();
    public DbSet<RentedTypeEntity> RentedTypes => Set<RentedTypeEntity>();
    public DbSet<PersonCage> Cages => Set<PersonCage>();
    public DbSet<Advertisement> Advertisements => Set<Advertisement>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<RentedCage> RentedCages => Set<RentedCage>();
    public DbSet<PriceAdjustmentWorkflow> PriceAdjustmentWorkflows => Set<PriceAdjustmentWorkflow>();
    public DbSet<PriceTypeWorkflow> PriceTypeWorkflows => Set<PriceTypeWorkflow>();
    public DbSet<Price> Prices => Set<Price>();
    public DbSet<Amounts> Amounts => Set<Amounts>();
    public DatabaseFacade DatabaseFacade => Database;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<RegistrationToExhibition> registrationsToExhibitionToBeDeleted = ChangeTracker
            .Entries<RegistrationToExhibition>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (RegistrationToExhibition registrationToExhibition in registrationsToExhibitionToBeDeleted)
        {
            foreach (CatRegistration catRegistration in registrationToExhibition.CatRegistrations)
            {
                Entry(catRegistration).State = EntityState.Deleted;
            }

            foreach (PersonCage cages in registrationToExhibition.Cages)
            {
                Entry(cages).State = EntityState.Deleted;
            }
        }

        List<CatRegistration> catRegistrationsToBeDeleted = ChangeTracker.Entries<CatRegistration>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (CatRegistration catRegistrationToDelete in catRegistrationsToBeDeleted)
        {
            if (catRegistrationToDelete.ExhibitedCat != null)
            {
                Entry(catRegistrationToDelete.ExhibitedCat).State = EntityState.Deleted;
            }

            if (catRegistrationToDelete.Litter != null)
            {
                Entry(catRegistrationToDelete.Litter).State = EntityState.Deleted;
            }
        }

        List<ExhibitedCat> exhibitedCatsToDelete = ChangeTracker.Entries<ExhibitedCat>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (ExhibitedCat exhibitedCat in exhibitedCatsToDelete)
        {
            if (exhibitedCat.Father != null)
            {
                Entry(exhibitedCat.Father).State = EntityState.Deleted;
            }

            if (exhibitedCat.Mother != null)
            {
                Entry(exhibitedCat.Mother).State = EntityState.Deleted;
            }

            if (exhibitedCat.Breeder != null)
            {
                Entry(exhibitedCat.Breeder).State = EntityState.Deleted;
            }
        }

        List<Litter> littersToDelete = ChangeTracker.Entries<Litter>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        foreach (Litter litter in littersToDelete)
        {
            Entry(litter.Father).State = EntityState.Deleted;
            Entry(litter.Mother).State = EntityState.Deleted;
            Entry(litter.Breeder).State = EntityState.Deleted;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (IMutableForeignKey relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        builder.Entity<ApplicationUser>()
            .Property(b => b.PhoneNumber)
            .IsRequired();

        builder.Entity<ApplicationUser>()
            .Property(b => b.Email)
            .IsRequired();
        builder.Entity<ApplicationUser>()
            .HasIndex(b => b.Email)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .Property(b => b.UserName)
            .IsRequired();
        builder.Entity<ApplicationUser>()
            .HasIndex(b => b.UserName)
            .IsUnique();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
