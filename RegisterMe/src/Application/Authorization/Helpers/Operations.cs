#region

using Microsoft.AspNetCore.Authorization.Infrastructure;

#endregion

namespace RegisterMe.Application.Authorization.Helpers;

public static class Operations
{
    public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };

    public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };

    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };

    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };

    public static readonly OperationAuthorizationRequirement OnlyOwnerCanDo = new() { Name = nameof(OnlyOwnerCanDo) };

    public static readonly OperationAuthorizationRequirement DoSuperAdminStuff =
        new() { Name = nameof(DoSuperAdminStuff) };

    public static readonly OperationAuthorizationRequirement DoOrganizationAdminStuff =
        new() { Name = nameof(DoOrganizationAdminStuff) };
}
