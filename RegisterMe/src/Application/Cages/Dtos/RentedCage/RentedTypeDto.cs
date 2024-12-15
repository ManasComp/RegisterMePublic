#region

using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages.Dtos.RentedCage;

public class RentedTypeDto
{
    public required int Id { get; set; }
    public required RentedType RentedType { get; set; }
}
