#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Exceptions;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.CreateRentedCage;

// ReSharper disable always UnusedType.Global
public record AddNewRentedCageGroupToExhibitionCommand : IRequest<Result<string>>
{
    public required CreateRentedRentedCageDto CreateRentedRentedCageDto { get; init; }
}

public class
    AddNewRentedCageGroupToExhibitionCommandValidator : AbstractValidator<AddNewRentedCageGroupToExhibitionCommand>
{
    public AddNewRentedCageGroupToExhibitionCommandValidator()
    {
        RuleFor(v => v.CreateRentedRentedCageDto).NotEmpty();
        RuleFor(v => v.CreateRentedRentedCageDto.Length)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(500);
        RuleFor(v => v.CreateRentedRentedCageDto.Height)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(500);
        RuleFor(v => v.CreateRentedRentedCageDto.Width)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(500);
        RuleFor(v => v.CreateRentedRentedCageDto.RentedCageTypes)
            .NotEmpty()
            .Must(x => x.Count > 0);
        RuleFor(v => v.CreateRentedRentedCageDto.RentedCageTypes)
            .Must(x => x.TrueForAll(item => Enum.IsDefined(typeof(RentedType), item)))
            .WithMessage("All rented cage types must be defined in the RentedType enum.");
        RuleFor(v => v.CreateRentedRentedCageDto.ExhibitionDaysId)
            .NotEmpty()
            .Must(x => x.Count > 0).ForEach(x => x.ForeignKeyValidator());
        RuleFor(x => x.CreateRentedRentedCageDto.Count)
            .GreaterThan(0)
            .LessThan(500);
    }
}

public class AddNewRentedCageGroupToExhibitionCommandHandler(
    ICagesService cagesService,
    IAuthorizationService authorizationService,
    IUser user,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<AddNewRentedCageGroupToExhibitionCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddNewRentedCageGroupToExhibitionCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CreateRentedRentedCageDto.ExhibitionDaysId.Count == 0)
        {
            return Result.Failure<string>(Errors.ThereAreZeroExhibitedDaysError);
        }

        int exhibitionDayId = request.CreateRentedRentedCageDto.ExhibitionDaysId[0];
        int? exhibitionId = await applicationDbContext.ExhibitionDays.Where(x => x.Id == exhibitionDayId)
            .Select(x => x.ExhibitionId).FirstOrDefaultAsync(cancellationToken);
        if (exhibitionId is null or 0)
        {
            throw new NotFoundException(nameof(exhibitionId), "exhibitionId");
        }

        AuthorizationResult result = await authorizationService.AuthorizeAsync(
            AuthorizationHelperMethods.ThrowExceptionIfUserNotLoggedIn(user),
            new AuthorizeExhibitionId(exhibitionId), Operations.Update);
        Guard.Against.UnAuthorized(result);
        Result<string> rentedCageIds =
            await cagesService.CreateRentedCages(request.CreateRentedRentedCageDto, cancellationToken);

        return rentedCageIds;
    }
}
