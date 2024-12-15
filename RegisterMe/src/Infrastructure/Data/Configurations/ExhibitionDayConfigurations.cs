#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class ExhibitionDayConfiguration : IEntityTypeConfiguration<ExhibitionDay>
{
    public void Configure(EntityTypeBuilder<ExhibitionDay> builder)
    {
        builder.HasMany(x => x.Prices)
            .WithMany(x => x.ExhibitionDays)
            .UsingEntity<Dictionary<string, object>>(
                "ExhibitionDayPriceJoinTable",
                j => j
                    .HasOne<Price>()
                    .WithMany()
                    .HasForeignKey("PriceId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<ExhibitionDay>()
                    .WithMany()
                    .HasForeignKey("ExhibitionDayId")
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}
