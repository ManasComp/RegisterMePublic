#region

using RegisterMe.Application.Exhibitions.Dtos;
using RegisterMe.Application.Services.Workflows;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class CreateExhibitionModel
{
    public int ExhibitionId { get; init; }
    public required string Name { get; init; } = null!;
    public required string Email { get; init; } = null!;
    public required string Phone { get; init; } = null!;
    public required string Url { get; init; } = null!;
    public required string Description { get; init; } = null!;
    public required string NormalAccount { get; init; } = null!;
    public required string Iban { get; init; } = null!;
    public required DateOnly RegistrationStart { get; init; }
    public required DateOnly RegistrationEnd { get; init; }
    public required DateOnly ExhibitionStart { get; init; }
    public required DateOnly ExhibitionEnd { get; init; }
    public required int OrganizationId { get; init; }
    public required int DeleteNotFinishedRegistrationsAfterHours { get; init; }
    public required AddressDto Address { get; init; }

    public static CreateExhibitionModel CreateBlank(int organizationId)
    {
        DateOnly actualDate = DateOnly.FromDateTime(DateTime.Now);
        return new CreateExhibitionModel
        {
            Name = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            Url = string.Empty,
            Description = string.Empty,
            NormalAccount = string.Empty,
            Iban = string.Empty,
            RegistrationStart = actualDate.AddDays(-1),
            RegistrationEnd = actualDate.AddDays(30),
            ExhibitionStart = actualDate.AddDays(37),
            ExhibitionEnd = actualDate.AddDays(38),
            OrganizationId = organizationId,
            Address = new AddressDto { Latitude = "", StreetAddress = "", Longitude = "" },
            DeleteNotFinishedRegistrationsAfterHours = 24
        };
    }

    public static CreateExhibitionModel FromExhibition(BriefExhibitionDto exhibitionDto)
    {
        return new CreateExhibitionModel
        {
            ExhibitionId = exhibitionDto.Id,
            Name = exhibitionDto.Name,
            Email = exhibitionDto.Email,
            Phone = exhibitionDto.Phone,
            Url = exhibitionDto.Url,
            Description = exhibitionDto.Description,
            NormalAccount = exhibitionDto.BankAccount,
            Iban = exhibitionDto.Iban,
            RegistrationStart = exhibitionDto.RegistrationStart,
            RegistrationEnd = exhibitionDto.RegistrationEnd,
            ExhibitionStart = exhibitionDto.ExhibitionStart,
            ExhibitionEnd = exhibitionDto.ExhibitionEnd,
            OrganizationId = exhibitionDto.OrganizationId,
            Address = exhibitionDto.Address,
            DeleteNotFinishedRegistrationsAfterHours = exhibitionDto.DeleteNotFinishedRegistrationsAfterHours
        };
    }
}
