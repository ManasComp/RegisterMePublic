#region

using System.Reflection;
using Microsoft.AspNetCore.Identity;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Exhibitors.Dtos;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Constants;
using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.Users;

public class UserService(IApplicationDbContext appContext, UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<Result> DeletePersonalDataAsync(string userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<PropertyInfo> exhibitorAndUserPersonalDataProps = typeof(ExhibitorAndUserDto).GetProperties().Where(
            prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));

        Dictionary<string, string> personalData = new() { { "UserName", "Deleted" + Guid.NewGuid() } };
        foreach (PropertyInfo p in exhibitorAndUserPersonalDataProps)
        {
            personalData.Add(p.Name, "Deleted");
        }

        HashSet<string> personalDataPropsNameSet = personalData.Keys.ToHashSet();
        Exhibitor? exhibitor = await appContext.Exhibitors.Where(x => x.AspNetUserId == userId)
            .Include(x => x.AspNetUser)
            .SingleOrDefaultAsync(cancellationToken);

        if (exhibitor != null)
        {
            bool isInActiveRegistration = appContext.RegistrationsToExhibition
                .Where(x => x.ExhibitorId == exhibitor.Id)
                .Include(x => x.Exhibition)
                .ThenInclude(e => e.Days)
                .AsEnumerable()
                .Select(x => x.Exhibition)
                .Any(e => e.RegistrationStart > DateOnly.FromDateTime(DateTime.Now) &&
                          e.Days.Count != 0 &&
                          e.Days.Max(day => day.Date) < DateOnly.FromDateTime(DateTime.Now));
            if (isInActiveRegistration)
            {
                return Result.Failure(Errors.CannotDeletePersonalDataWhileInActiveRegistrationError);
            }
        }

        if (await appContext.Organizations.AnyAsync(
                x => x.Administrator.Select(applicationUser => applicationUser.Id).Contains(userId),
                cancellationToken))
        {
            return Result.Failure(Errors.CannotDeletePersonalDataIfYouAreOrganiationAdminError);
        }

        ApplicationUser? user = await userManager.FindByIdAsync(userId);
        Guard.Against.Null(user, nameof(user));
        if (await userManager.IsInRoleAsync(user, Roles.Administrator))
        {
            return Result.Failure(Errors.CannotDeletePersonalDataIfYouAreAdminError);
        }

        if (exhibitor != null)
        {
            IEnumerable<PropertyInfo> exhibitorProps = typeof(Exhibitor).GetProperties().Where(
                prop => personalDataPropsNameSet.Contains(prop.Name));
            foreach (PropertyInfo p in exhibitorProps)
            {
                p.SetValue(exhibitor, personalData[p.Name]);
            }
        }

        IEnumerable<PropertyInfo> applicationProps = typeof(ApplicationUser).GetProperties().Where(
            prop => personalDataPropsNameSet.Contains(prop.Name));
        foreach (PropertyInfo p in applicationProps)
        {
            if (p.Name == "DateOfBirth")
            {
                p.SetValue(user, new DateOnly(1900, 1, 1));
                continue;
            }

            p.SetValue(user, personalData[p.Name]);
        }

        await userManager.RemovePasswordAsync(user);
        await userManager.UpdateAsync(user);
        if (exhibitor != null)
        {
            appContext.Exhibitors.Update(exhibitor);
        }

        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
