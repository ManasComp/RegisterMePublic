#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class RegistrationToExhibitionConfiguration : IEntityTypeConfiguration<RegistrationToExhibition>
{
    public void Configure(EntityTypeBuilder<RegistrationToExhibition> builder)
    {
        builder.HasMany(x => x.CatRegistrations)
            .WithOne(x => x.RegistrationToExhibition)
            .HasForeignKey(x => x.RegistrationToExhibitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PersonRegistration)
            .WithOne(x => x.RegistrationToExhibition)
            .HasForeignKey<PersonRegistration>(x => x.RegistrationToExhibitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Cages)
            .WithOne(x => x.RegistrationToExhibition)
            .HasForeignKey(x => x.RegistrationToExhibitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PaymentInfo)
            .WithOne(x => x.RegistrationToExhibition)
            .HasForeignKey<PaymentInfo>(x => x.RegistrationToExhibitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Advertisement)
            .WithMany(x => x.PersonRegistrations)
            .HasForeignKey(x => x.AdvertisementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ExhibitionId, x.ExhibitorId }).IsUnique();
    }
}
