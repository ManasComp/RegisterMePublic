#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.HasMany(x => x.Amounts)
            .WithOne(x => x.Advertisement)
            .HasForeignKey(x => x.AdvertisementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Amounts).AutoInclude();
    }
}
