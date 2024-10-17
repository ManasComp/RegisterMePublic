#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace RegisterMe.Domain.Entities;

public class Group
{
    public required string Name { get; init; } = null!;
    [Key] public required string GroupId { get; init; } = null!;
    public virtual List<CatDay> CatDays { get; init; } = [];
    public virtual List<Price> Prices { get; init; } = [];
}
