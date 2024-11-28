#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class LitterConfiguration : IEntityTypeConfiguration<Litter>
{
    public void Configure(EntityTypeBuilder<Litter> builder)
    {
        builder.ToTable("Litters");
        builder.HasOne(b => b.Father).WithOne().HasForeignKey<Parent>(x => x.LitterIsFatherOfId)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(b => b.Mother).WithOne().HasForeignKey<Parent>(x => x.LitterIsMotherOfId)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(b => b.Breeder).WithOne(b => b.Litter).HasForeignKey<Breeder>(b => b.LitterId)
            .OnDelete(DeleteBehavior.ClientCascade);


        builder.Navigation(b => b.Father).AutoInclude();
        builder.Navigation(b => b.Mother).AutoInclude();
        builder.Navigation(b => b.Breeder).AutoInclude();
    }
}
