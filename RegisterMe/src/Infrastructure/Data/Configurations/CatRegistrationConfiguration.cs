#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class CatRegistrationConfiguration : IEntityTypeConfiguration<CatRegistration>
{
    public void Configure(EntityTypeBuilder<CatRegistration> builder)
    {
        builder.HasMany(x => x.CatDays)
            .WithOne(x => x.CatRegistration)
            .HasForeignKey(x => x.CatRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ExhibitedCat)
            .WithOne(x => x.CatRegistration)
            .HasForeignKey<ExhibitedCat>(x => x.CatRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Litter)
            .WithOne(x => x.CatRegistration)
            .HasForeignKey<Litter>(x => x.CatRegistrationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
