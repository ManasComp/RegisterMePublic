#region

using Microsoft.AspNetCore.Authorization;
using RegisterMe.Application.Authorization;
using RegisterMe.Application.Authorization.Helpers;
using RegisterMe.Application.Cages;
using RegisterMe.Application.Cages.Dtos.RentedCage;
using RegisterMe.Application.Common.Interfaces;
using RegisterMe.Application.Common.Validators;
using RegisterMe.Application.Exceptions;
using RegisterMe.Domain.Common;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Exhibitions.Commands.UpdateRentedCageGroupDto;

// ReSharper disable always UnusedType.Global
public record UpdateRentedCageGroupCommand : IRequest<Result<string>>
{
    public required string CagesId { get; init; }
    public required CreateRentedRentedCageDto RentedCage { get; init; }
}

public class UpdateRentedCageGroupDtoCommandValidator : AbstractValidator<UpdateRentedCageGroupCommand>
{
    public UpdateRentedCageGroupDtoCommandValidator()
    {
        RuleFor(v => v.RentedCage).NotEmpty();
        RuleFor(v => v.RentedCage.Length)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(500);
        RuleFor(v => v.RentedCage.Height)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(500);
        RuleFor(v => v.RentedCage.Width)
            .GreaterThanOrEqualTo(50)
            .LessThanOrEqualTo(500);
        RuleFor(v => v.RentedCage.RentedCageTypes)
            .NotEmpty()
            .Must(x => x.Count > 0);
        RuleFor(v => v.RentedCage.RentedCageTypes)
            .Must(x => x.TrueForAll(item => Enum.IsDefined(typeof(RentedType), item)))
            .WithMessage("All rented cage types must be defined in the RentedType enum.");
        RuleFor(v => v.RentedCage.ExhibitionDaysId)
            .NotEmpty()
            .Must(x => x.Count > 0).ForEach(x => x.ForeignKeyValidator());
        RuleFor(x => x.RentedCage.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(500);
    }
}

public class UpdateRentedCageGroupDtoCommandHandler(
    ICagesService cagesService,
    IUser user,
    IAuthorizationService authorizationService,
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<UpdateRentedCageGroupCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateRentedCageGroupCommand request,
        CancellationToken cancellationToken)
    {
        int exhibitionDayId = request.RentedCage.ExhibitionDaysId[0];
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
        Result<string> data =
            await cagesService.UpdateCageGroup(request.CagesId, request.RentedCage, cancellationToken);
        return data;
    }
}
