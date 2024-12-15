#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasMany(x => x.CatDays)
            .WithMany(x => x.Groups)
            .UsingEntity("CatDayToGroupsJoinTable");

        builder.HasMany(x => x.Prices)
            .WithMany(x => x.Groups)
            .UsingEntity("PriceToGroupsJoinTable");
    }
}
