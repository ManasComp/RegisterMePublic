#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class PriceConfiguration : IEntityTypeConfiguration<Price>
{
    public void Configure(EntityTypeBuilder<Price> builder)
    {
        builder.HasMany(x => x.Amounts)
            .WithOne(x => x.Price)
            .HasForeignKey(x => x.PriceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Amounts).AutoInclude();
    }
}
