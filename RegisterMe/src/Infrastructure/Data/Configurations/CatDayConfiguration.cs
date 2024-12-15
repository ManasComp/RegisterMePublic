#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class CatDayRegistrationConfiguration : IEntityTypeConfiguration<CatDay>
{
    public void Configure(EntityTypeBuilder<CatDay> builder)
    {
        builder.HasOne(x => x.Cage)
            .WithMany(x => x.CatDays)
            .HasForeignKey(x => x.ExhibitorsCage)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RentedTypeEntity)
            .WithMany(x => x.CatDays)
            .HasForeignKey(x => x.RentedCageTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
