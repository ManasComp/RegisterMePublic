#region

using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.RegistrationToExhibition.Dtos;
using RegisterMe.Application.Services.Ems;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Entities;
using RegisterMe.Domain.Enums;
using Type = RegisterMe.Application.RegistrationToExhibition.Enums.Type;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition;

public class RegistrationToExhibitionService(
    IApplicationDbContext appContext,
    IMapper mapper,
    UserManager<ApplicationUser> userManager,
    TimeProvider timeProvider) : IRegistrationToExhibitionService
{
    public static readonly Expression<Func<Domain.Entities.RegistrationToExhibition, bool>> WasPaid =
        registrationToExhibition =>
            registrationToExhibition.PaymentInfo != null &&
            (registrationToExhibition.PaymentInfo.PaymentType != PaymentType.PayOnlineByCard ||
             registrationToExhibition.PaymentInfo.PaymentCompletedDate != null);

    public static readonly Expression<Func<Domain.Entities.RegistrationToExhibition, bool>> WasNotPaid =
        registrationToExhibition =>
            registrationToExhibition.PaymentInfo == null ||
            (registrationToExhibition.PaymentInfo.PaymentType == PaymentType.PayOnlineByCard &&
             registrationToExhibition.PaymentInfo.PaymentCompletedDate == null);

    public async Task<Result<int>> CreateRegistrationToExhibition(
        CreateRegistrationToExhibitionDto registrationToExhibitionDto,
        CancellationToken cancellationToken = default)
    {
        Exhibitor? exhibitor =
            await appContext.Exhibitors.FindAsync([registrationToExhibitionDto.ExhibitorId], cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionDto.ExhibitorId, exhibitor);
        ApplicationUser? user = await userManager.FindByIdAsync(exhibitor.AspNetUserId);
        Guard.Against.NotFound(exhibitor.AspNetUserId, user);
        Domain.Entities.RegistrationToExhibition registrationToExhibition = new()
        {
            AdvertisementId = registrationToExhibitionDto.AdvertisementId,
            ExhibitionId = registrationToExhibitionDto.ExhibitionId,
            ExhibitorId = registrationToExhibitionDto.ExhibitorId,
            PersonRegistration = new PersonRegistration
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                City = exhibitor.City,
                ZipCode = exhibitor.ZipCode,
                Country = exhibitor.Country,
                Street = exhibitor.Street,
                HouseNumber = exhibitor.HouseNumber,
                DateOfBirth = user.DateOfBirth,
                MemberNumber = exhibitor.MemberNumber,
                Organization = exhibitor.Organization,
                PhoneNumber = user.PhoneNumber!,
                IsPartOfCsch = exhibitor.IsPartOfCsch,
                IsPartOfFife = exhibitor.IsPartOfFife,
                EmailToOrganization = exhibitor.EmailToOrganization
            }
        };
        Exhibition? exhibition =
            await appContext.Exhibitions.FindAsync([registrationToExhibitionDto.ExhibitionId], cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionDto.ExhibitionId, exhibition);
        if (exhibition.IsCancelled)
        {
            return Result.Failure<int>(Errors.ExhibitionIsCancelledError);
        }

        if (!exhibition.IsPublished)
        {
            return Result.Failure<int>(Errors.ExhibitionIsNotPublishedError);
        }

        bool registrationsToExhibition = await appContext.RegistrationsToExhibition
            .Where(x => x.ExhibitionId == registrationToExhibitionDto.ExhibitionId &&
                        x.ExhibitorId == registrationToExhibitionDto.ExhibitorId)
            .AnyAsync(cancellationToken);
        if (registrationsToExhibition)
        {
            return Result.Failure<int>(Errors.ExhibitorAlreadyRegisteredError);
        }

        if (exhibition.RegistrationEnd < DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
        {
            return Result.Failure<int>(Errors.RegistrationIsCloseError);
        }

        if (exhibition.RegistrationStart > DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime))
        {
            return Result.Failure<int>(Errors.RegistrationHasNotStartedError);
        }

        appContext.RegistrationsToExhibition.Add(registrationToExhibition);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success(registrationToExhibition.Id);
    }

    public async Task<Result> DeleteRegistration(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.RegistrationToExhibition? catRegistration =
            await appContext.RegistrationsToExhibition
                .Include(x => x.CatRegistrations)
                .ThenInclude(x => x.ExhibitedCat)
                .Include(x => x.CatRegistrations)
                .ThenInclude(x => x.Litter)
                .Where(x => x.Id == registrationToExhibitionId)
                .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionId, catRegistration);

        appContext.CatRegistrations.RemoveRange(catRegistration.CatRegistrations);

        await appContext.SaveChangesAsync(cancellationToken);
        appContext.RegistrationsToExhibition.Remove(catRegistration);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<List<TypedEmail>>> DeleteTemporaryRegistrations(int? exhibitionId,
        CancellationToken cancellationToken = default)
    {
        DateTimeOffset actualTime = timeProvider.GetLocalNow().ToUniversalTime();
        IQueryable<Domain.Entities.RegistrationToExhibition> query =
            appContext.RegistrationsToExhibition
                .Include(x => x.PaymentInfo)
                .Include(x => x.Exhibition)
                .Include(x => x.PersonRegistration)
                .Include(x => x.CatRegistrations)
                .ThenInclude(x => x.ExhibitedCat)
                .Include(x => x.CatRegistrations)
                .ThenInclude(x => x.Litter)
                .Where(WasNotPaid)
                .AsQueryable();

        if (exhibitionId.HasValue)
        {
            query = query.Where(x => x.ExhibitionId == exhibitionId);
        }
        else
        {
            query = query.Where(x =>
                    x.Exhibition.RegistrationEnd.ToDateTime(new TimeOnly())
                        .AddHours(x.Exhibition.DeleteNotFinishedRegistrationsAfterHours) >= actualTime)
                .Where(x => x.Created.AddHours(x.Exhibition.DeleteNotFinishedRegistrationsAfterHours) < actualTime);
        }

        List<Domain.Entities.RegistrationToExhibition> registrationsToExhibitions =
            await query.ToListAsync(cancellationToken);

        List<TypedEmail> emails = registrationsToExhibitions.Select(x => new TypedEmail(x.PersonRegistration.Email,
            Type.IsDeleted, x.ExhibitionId, timeProvider.GetLocalNow().ToUniversalTime())).ToList();

        appContext.CatRegistrations.RemoveRange(registrationsToExhibitions.SelectMany(x => x.CatRegistrations));
        await appContext.SaveChangesAsync(cancellationToken);
        appContext.RegistrationsToExhibition.RemoveRange(registrationsToExhibitions);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success(emails);
    }


    public async Task<List<TypedEmail>> GetTemporaryRegistrationsEmails(CancellationToken cancellationToken = default)
    {
        DateTimeOffset actualTime = timeProvider.GetLocalNow().ToUniversalTime();
        List<TypedEmail> query = await
            appContext.RegistrationsToExhibition
                .Include(x => x.PaymentInfo)
                .Include(x => x.Exhibition)
                .Where(x =>
                    x.Exhibition.RegistrationEnd.ToDateTime(new TimeOnly())
                        .AddHours(x.Exhibition.DeleteNotFinishedRegistrationsAfterHours) >= actualTime)
                .Where(x => x.Created.AddHours(
                    Math.Ceiling(x.Exhibition.DeleteNotFinishedRegistrationsAfterHours / 2.0)) < actualTime)
                .Where(WasNotPaid)
                .Select(x => new TypedEmail(
                    x.PersonRegistration.Email,
                    x.Created.AddHours(x.Exhibition.DeleteNotFinishedRegistrationsAfterHours) < actualTime
                        ? Type.IsDeleted
                        : Type.CanBeDeletedInFuture, x.ExhibitionId,
                    x.LastNotificationSendOn)
                )
                .ToListAsync(cancellationToken);

        return query;
    }

    public async Task<RegistrationToExhibitionDto> GetRegistrationToExhibitionById(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        RegistrationToExhibitionDto? dto = await appContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Include(x => x.PersonRegistration)
            .Include(x => x.Exhibitor)
            .Include(x => x.PaymentInfo)
            .Include(x => x.CatRegistrations)
            .Select(x => mapper.Map<RegistrationToExhibitionDto>(x))
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(registrationToExhibitionId, dto);
        return dto;
    }

    public async Task<RegistrationToExhibitionDto?> GetRegistrationToExhibitionByExhibitorIdAndExhibitionId(
        int exhibitionId, int exhibitorId, CancellationToken cancellationToken = default)
    {
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext.RegistrationsToExhibition
            .Where(x => x.ExhibitionId == exhibitionId && x.ExhibitorId == exhibitorId)
            .Include(x => x.PersonRegistration)
            .Include(x => x.Exhibitor)
            .Include(x => x.CatRegistrations)
            .Include(x => x.PaymentInfo)
            .SingleOrDefaultAsync(cancellationToken);

        RegistrationToExhibitionDto? dto =
            mapper.Map<RegistrationToExhibitionDto?>(registrationToExhibition);

        return dto;
    }

    public async Task<Result> UpdateSendNotifications(List<SimpleTypedEmail> emails,
        CancellationToken cancellationToken = default)
    {
        List<Domain.Entities.RegistrationToExhibition> allRegistrations = await appContext.RegistrationsToExhibition
            .Include(x => x.Exhibitor)
            .ThenInclude(x => x.AspNetUser)
            .ToListAsync(cancellationToken);

        List<Domain.Entities.RegistrationToExhibition> data = allRegistrations.Where(x => emails
                .Any(e => e.ExhibitionId == x.ExhibitionId && e.Email == x.Exhibitor.AspNetUser.Email))
            .ToList();

        data.ForEach(x => x.LastNotificationSendOn = timeProvider.GetLocalNow().ToUniversalTime());

        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeAdvertisement(int advertisementId, int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext.RegistrationsToExhibition
            .FindAsync([registrationToExhibitionId, cancellationToken], cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionId, registrationToExhibition);

        registrationToExhibition.AdvertisementId = advertisementId;
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<List<RegistrationToExhibitionDto>> GetRegistrationsToExhibitionByExhibitorId(int exhibitorId,
        CancellationToken cancellationToken = default)
    {
        List<Domain.Entities.RegistrationToExhibition> registrationToExhibition = await appContext
            .RegistrationsToExhibition
            .Where(x => x.ExhibitorId == exhibitorId)
            .Include(x => x.PersonRegistration)
            .Include(x => x.Exhibitor)
            .Include(x => x.PaymentInfo)
            .Include(x => x.CatRegistrations)
            .ToListAsync(cancellationToken);

        List<RegistrationToExhibitionDto>? dto =
            mapper.Map<List<RegistrationToExhibitionDto>>(registrationToExhibition);

        return dto;
    }

    public async Task<bool> HasValidEms(int registrationToExhibitionId, CancellationToken cancellationToken = default)
    {
        var ems = await appContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Include(x => x.CatRegistrations)
            .ThenInclude(x => x.ExhibitedCat)
            .SelectMany(x => x.CatRegistrations)
            .Where(x => x.ExhibitedCat != null)
            .Select(x => new
            {
                ems = EmsCode.Create(x.ExhibitedCat!.Ems),
                breed = x.ExhibitedCat!.Breed,
                colour = x.ExhibitedCat!.Colour
            })
            .ToListAsync(cancellationToken);

        if (ems.Select(x => x.ems).Any(x => x.IsFailure))
        {
            return false;
        }

        return ems.TrueForAll(x => x.ems.Value.VerifyEmsCode(x.breed, x.colour));
    }


    public async Task<List<RegistrationToExhibitionDto>> GetRegistrationsToExhibitionByExhibitionId(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        List<Domain.Entities.RegistrationToExhibition> registrationToExhibition = await appContext
            .RegistrationsToExhibition
            .Where(x => x.ExhibitionId == exhibitionId)
            .Include(x => x.PaymentInfo)
            .Where(WasPaid)
            .Include(x => x.PersonRegistration)
            .Include(x => x.Exhibitor)
            .Include(x => x.CatRegistrations)
            .ToListAsync(cancellationToken);

        List<RegistrationToExhibitionDto>? dto =
            mapper.Map<List<RegistrationToExhibitionDto>>(registrationToExhibition);

        return dto;
    }


    public Task<bool> HasActiveRegistrations(int exhibitionId, string userId, bool? paid,
        CancellationToken cancellationToken = default)
    {
        bool isRegistered = appContext.RegistrationsToExhibition
            .Where(x => x.ExhibitionId == exhibitionId && x.Exhibitor.AspNetUserId == userId)
            .Include(x => x.PaymentInfo)
            .AsEnumerable()
            .Any(x => paid == null || paid == IsPaid(x.PaymentInfo));

        return Task.FromResult(isRegistered);
    }

    public async Task<bool> HasDrafts(int exhibitionId,
        CancellationToken cancellationToken = default)
    {
        bool hasDrafts = await appContext.RegistrationsToExhibition
            .Where(x => x.ExhibitionId == exhibitionId)
            .AnyAsync(WasNotPaid, cancellationToken);
        return hasDrafts;
    }


    public async Task<Result> StartOnlinePayment(int registrationToExhibitionId, string sessionId, Currency currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sessionId);
        Domain.Entities.RegistrationToExhibition registrationToExhibition = await
            appContext.RegistrationsToExhibition.Where(x => x.Id == registrationToExhibitionId)
                .Include(x => x.PaymentInfo)
                .SingleAsync(cancellationToken);
        if (registrationToExhibition.CatRegistrations.Count == 0)
        {
            return Result.Failure(Errors.NoCatsRegisteredError);
        }

        if (IsPaid(registrationToExhibition.PaymentInfo))
        {
            return Result.Failure(Errors.PaymentInfoAlreadyExistsError);
        }

        if (registrationToExhibition.PaymentInfo == null)
        {
            PaymentInfo payInformation = new()
            {
                PaymentRequestDate = timeProvider.GetLocalNow().ToUniversalTime(),
                RegistrationToExhibitionId = registrationToExhibitionId,
                PaymentType = PaymentType.PayOnlineByCard,
                SessionId = sessionId,
                Amounts = new Amounts { Amount = amount, Currency = currency }
            };

            registrationToExhibition.PaymentInfo = payInformation;
        }
        else
        {
            appContext.Amounts.Remove(registrationToExhibition.PaymentInfo.Amounts);
            registrationToExhibition.PaymentInfo.PaymentRequestDate = timeProvider.GetLocalNow().ToUniversalTime();
            registrationToExhibition.PaymentInfo.PaymentType = PaymentType.PayOnlineByCard;
            registrationToExhibition.PaymentInfo.SessionId = sessionId;
            registrationToExhibition.PaymentInfo.Amounts = new Amounts { Amount = amount, Currency = currency };
        }

        appContext.PaymentInfos.Update(registrationToExhibition.PaymentInfo);
        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


    public async Task<Result> FinishOnlinePayment(int registrationToExhibitionId, string sessionId,
        string paymentIntentId, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrEmpty(sessionId);

        Domain.Entities.RegistrationToExhibition? registrationToExhibition = appContext.RegistrationsToExhibition
            .SingleOrDefault(x => x.Id == registrationToExhibitionId);
        Guard.Against.NotFound(registrationToExhibitionId, registrationToExhibition);
        if (registrationToExhibition.PaymentInfo == null)
        {
            return Result.Failure(Errors.PaymentInfoNotFoundError);
        }

        registrationToExhibition.PaymentInfo.SessionId = sessionId;
        registrationToExhibition.PaymentInfo.PaymentIntentId = paymentIntentId;
        registrationToExhibition.PaymentInfo.PaymentCompletedDate = timeProvider.GetLocalNow().ToUniversalTime();
        appContext.PaymentInfos.Update(registrationToExhibition.PaymentInfo);

        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


    public async Task<Result> RequestDelayedPayment(int registrationToExhibitionId, PaymentType paymentType,
        Currency currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        if (paymentType == PaymentType.PayOnlineByCard)
        {
            throw new InvalidEnumArgumentException();
        }

        Domain.Entities.RegistrationToExhibition registrationToExhibition = await
            appContext.RegistrationsToExhibition.Where(x => x.Id == registrationToExhibitionId)
                .Include(x => x.PaymentInfo)
                .SingleAsync(cancellationToken);

        if (registrationToExhibition.CatRegistrations.Count == 0)
        {
            return Result.Failure(Errors.NoCatsRegisteredError);
        }

        if (IsPaid(registrationToExhibition.PaymentInfo))
        {
            return Result.Failure(Errors.PaymentInfoAlreadyExistsError);
        }

        PaymentInfo payInformation = new()
        {
            PaymentRequestDate = timeProvider.GetLocalNow().ToUniversalTime(),
            RegistrationToExhibitionId = registrationToExhibitionId,
            PaymentType = paymentType,
            Amounts = new Amounts { Amount = amount, Currency = currency }
        };

        appContext.PaymentInfos.Add(payInformation);

        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> FinishDelayedPayment(int registrationToExhibitionId,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Include(x => x.PaymentInfo)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionId, registrationToExhibition);
        Guard.Against.Null(registrationToExhibition.PaymentInfo);
        if (registrationToExhibition.PaymentInfo.PaymentCompletedDate != null)
        {
            return Result.Failure(Errors.PaymentAlreadyCompletedError);
        }

        registrationToExhibition.PaymentInfo.PaymentCompletedDate = timeProvider.GetLocalNow().ToUniversalTime();

        appContext.PaymentInfos.Update(registrationToExhibition.PaymentInfo);

        await appContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> BalanceThePayment(int registrationToExhibitionId, decimal price,
        CancellationToken cancellationToken = default)
    {
        Domain.Entities.RegistrationToExhibition? registrationToExhibition = await appContext.RegistrationsToExhibition
            .Where(x => x.Id == registrationToExhibitionId)
            .Include(x => x.PaymentInfo)
            .SingleOrDefaultAsync(cancellationToken);
        Guard.Against.NotFound(registrationToExhibitionId, registrationToExhibition);
        Guard.Against.Null(registrationToExhibition.PaymentInfo);
        if (registrationToExhibition.PaymentInfo.PaymentCompletedDate == null)
        {
            return Result.Failure(Errors.PaymentNotCompletedYetError);
        }

        registrationToExhibition.PaymentInfo.PaymentCompletedDate = timeProvider.GetLocalNow().ToUniversalTime();

        registrationToExhibition.PaymentInfo.Amounts.Amount = price;

        appContext.PaymentInfos.Update(registrationToExhibition.PaymentInfo);

        await appContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public static bool IsPaid(PaymentInfo? paymentInfo)
    {
        if (paymentInfo == null)
        {
            return false;
        }

        return paymentInfo.PaymentType != PaymentType.PayOnlineByCard || paymentInfo.PaymentCompletedDate != null;
    }

    public static bool IsPaid(PaymentInfoDto? paymentInfo)
    {
        if (paymentInfo == null)
        {
            return false;
        }

        return paymentInfo.PaymentType != PaymentType.PayOnlineByCard || paymentInfo.PaymentCompletedDate != null;
    }

    public static bool IsNotPaid(PaymentInfo? paymentInfo)
    {
        return !IsPaid(paymentInfo);
    }

    public static bool IsNotPaid(PaymentInfoDto? paymentInfo)
    {
        return !IsPaid(paymentInfo);
    }
}
