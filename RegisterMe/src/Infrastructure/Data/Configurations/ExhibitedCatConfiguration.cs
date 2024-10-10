#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class ExhibitedCatConfiguration : IEntityTypeConfiguration<ExhibitedCat>
{
    public void Configure(EntityTypeBuilder<ExhibitedCat> builder)
    {
        builder.ToTable("ExhibitedCats");
        builder.HasOne(b => b.Father).WithOne().HasForeignKey<Parent>(x => x.ExhibitedCatIsFatherOfId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.Mother).WithOne().HasForeignKey<Parent>(x => x.ExhibitedCatIsMotherOfId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.Breeder).WithOne(b => b.ExhibitedCat).HasForeignKey<Breeder>(b => b.ExhibitedCatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Father).AutoInclude();
        builder.Navigation(b => b.Mother).AutoInclude();
        builder.Navigation(b => b.Breeder).AutoInclude();
    }
}
