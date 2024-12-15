#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class RentedCageConfiguration : IEntityTypeConfiguration<RentedCage>
{
    public void Configure(EntityTypeBuilder<RentedCage> builder)
    {
        builder.HasMany(x => x.RentedTypes)
            .WithOne(x => x.RentedCage)
            .HasForeignKey(x => x.CageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ExhibitionDays)
            .WithMany(x => x.CagesForRent)
            .UsingEntity("RentedCageAndExhibitionDayJoinTable");
    }
}
