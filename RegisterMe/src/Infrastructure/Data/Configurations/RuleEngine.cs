#region

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RegisterMe.Domain.Entities.RulesEngine;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Infrastructure.Data.Configurations;

public class RuleEngine1Configuration : IEntityTypeConfiguration<MyRule>
{
    public void Configure(EntityTypeBuilder<MyRule> builder)
    {
        builder.HasOne<MyRule>().WithMany(r => r.Rules).HasForeignKey("RuleIDFK");

        JsonSerializerOptions serializationOptions = new(JsonSerializerDefaults.General);

        // this is ok because we rely on reflection here
        ValueComparer<Dictionary<string, object>> valueComparer = new(
            // ReSharper disable once UsageOfDefaultStructEquality
            (c1, c2) => c1!.SequenceEqual(c2!),
            // ReSharper disable once UsageOfDefaultStructEquality
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c);

        builder.Property(b => b.Properties)
            .HasConversion(
                v => JsonSerializer.Serialize(v, serializationOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, serializationOptions)!)
            .Metadata
            .SetValueComparer(valueComparer);

        builder.Property(p => p.Actions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, serializationOptions),
                v => JsonSerializer.Deserialize<RuleActions>(v, serializationOptions)!);


        builder.Ignore(b => b.WorkflowsToInject);
    }
}

public class RuleEngine2Configuration : IEntityTypeConfiguration<MyScopedParam>
{
    public void Configure(EntityTypeBuilder<MyScopedParam> builder)
    {
    }
}

public class RuleEngine3Configuration : IEntityTypeConfiguration<MyWorkflow>
{
    public void Configure(EntityTypeBuilder<MyWorkflow> builder)
    {
        builder.HasMany(r => r.Rules).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}

public class RuleEngine4Configuration : IEntityTypeConfiguration<PriceTypeWorkflow>
{
    public void Configure(EntityTypeBuilder<PriceTypeWorkflow> builder)
    {
        builder.HasMany(r => r.Rules).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}
