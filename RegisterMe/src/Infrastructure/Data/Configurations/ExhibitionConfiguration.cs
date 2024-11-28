#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Entities.RulesEngine;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class ExhibitionConfiguration : IEntityTypeConfiguration<Exhibition>
{
    public void Configure(EntityTypeBuilder<Exhibition> builder)
    {
        builder.HasMany(x => x.RegistrationsToExhibitions)
            .WithOne(x => x.Exhibition)
            .HasForeignKey(x => x.ExhibitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Days)
            .WithOne(x => x.Exhibition)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.ExhibitionId);

        builder.HasMany(x => x.Advertisements)
            .WithOne(x => x.Exhibition)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.ExhibitionId);

        builder.HasMany(x => x.Workflows)
            .WithOne(x => x.Exhibition)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(x => x.ExhibitionId);

        builder.HasOne(x => x.PaymentTypesWorkflow)
            .WithOne(x => x.Exhibition)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey<PriceTypeWorkflow>(x => x.ExhibitionId);

        builder.HasOne(x => x.Address)
            .WithOne(x => x.Exhibition)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey<Address>(x => x.ExhibitionId);
    }
}
