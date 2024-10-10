#region

using RegisterMe.Domain.Entities;

#endregion

namespace RegisterMe.Application.RegistrationToExhibition.Dtos;

public record PersonRegistrationDto
{
    /// <summary>
    ///     City
    /// </summary>
    public required string City { get; init; } = null!;

    /// <summary>
    ///     Street
    /// </summary>
    public required string Street { get; init; } = null!;

    /// <summary>
    ///     House number
    /// </summary>
    public required string HouseNumber { get; init; } = null!;

    /// <summary>
    ///     Zip code
    /// </summary>
    public required string ZipCode { get; init; } = null!;

    /// <summary>
    ///     Organization
    /// </summary>
    public required string Organization { get; init; }

    /// <summary>
    ///     Member number
    /// </summary>
    public required string MemberNumber { get; init; } = null!;

    /// <summary>
    ///     First name
    /// </summary>
    public required string FirstName { get; init; } = null!;

    /// <summary>
    ///     Last name
    /// </summary>
    public required string LastName { get; init; } = null!;

    /// <summary>
    ///     Country
    /// </summary>
    public required string Country { get; init; } = null!;

    /// <summary>
    ///     Date of birth
    /// </summary>
    public required DateOnly DateOfBirth { get; init; }

    /// <summary>
    ///     Email
    /// </summary>
    public required string Email { get; init; } = null!;

    /// <summary>
    ///     Phone number
    /// </summary>
    public required string PhoneNumber { get; init; } = null!;

    /// <summary>
    ///     Is part of csch
    /// </summary>
    public required bool IsPartOfCsch { get; init; }

    /// <summary>
    ///     Is part of fife
    /// </summary>
    public required bool IsPartOfFife { get; init; }

    /// <summary>
    ///     EMail to organization exhibitor is part of
    /// </summary>
    public required string EmailToOrganization { get; init; } = null!;

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PersonRegistration, PersonRegistrationDto>();

            CreateMap<PersonRegistrationDto, PersonRegistration>()
                .ForMember(x => x.RegistrationToExhibitionId, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.RegistrationToExhibition, opt => opt.Ignore());
        }
    }
}
