#region

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class PaymentInfoConfiguration : IEntityTypeConfiguration<PaymentInfo>
{
    public void Configure(EntityTypeBuilder<PaymentInfo> builder)
    {
        builder.HasOne(x => x.Amounts)
            .WithOne(x => x.PaymentInfo)
            .HasForeignKey<Amounts>(x => x.PaymentInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Amounts).AutoInclude();
    }
}
