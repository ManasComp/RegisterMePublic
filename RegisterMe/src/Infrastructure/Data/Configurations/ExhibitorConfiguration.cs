#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class ExhibitorConfiguration : IEntityTypeConfiguration<Exhibitor>
{
    public void Configure(EntityTypeBuilder<Exhibitor> builder)
    {
        builder.HasOne(x => x.AspNetUser)
            .WithOne(x => x.Exhibitor)
            .HasForeignKey<Exhibitor>(x => x.AspNetUserId);

        builder.HasMany(x => x.RegistrationToExhibitions)
            .WithOne(x => x.Exhibitor)
            .HasForeignKey(x => x.ExhibitorId);
    }
}
