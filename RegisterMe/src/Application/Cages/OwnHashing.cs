#region

using RegisterMe.Application.Cages.Dtos;
using RegisterMe.Application.Cages.Dtos.Cage;
using RegisterMe.Domain.Enums;

#endregion

namespace RegisterMe.Application.Cages;

public record RentedCageGroup
{
    public RentedCageGroup(int height, int length, int width, RentedType rentedType, RentingType rentingType)
    {
        CageGroupDescriptor = $"{height};{length};{width};{rentedType};{rentingType}";
    }

    public RentedCageGroup(string groupDescriptor)
    {
        Guard.Against.NullOrEmpty(groupDescriptor, nameof(groupDescriptor));
        try
        {
            Unparse(groupDescriptor);
        }
        catch (Exception e)
        {
            throw new ArgumentException("Invalid cage hash", e);
        }

        CageGroupDescriptor = groupDescriptor;
    }

    private string CageGroupDescriptor { get; }


    public string GetDescription()
    {
        CageGroupDescriptorAttributes unparsed = Unparse();
        string text = unparsed.CageType switch
        {
            RentingType.RentedWithOneOtherCat => "2 kočky v ",
            RentingType.RentedWithTwoOtherCats => "3 kočky v ",
            _ => ""
        };

        int length = unparsed.Length;
        switch (unparsed.RentedType)
        {
            case RentedType.Double:
                text += "Dvouklec ";
                break;
            case RentedType.Single:
                text += "Jednoklec ";
                length /= 2;
                break;
        }

        text += $"{length}x{unparsed.Width}";
        return text;
    }

    public CageGroupDescriptorAttributes Unparse()
    {
        return Unparse(CageGroupDescriptor);
    }

    private static CageGroupDescriptorAttributes Unparse(string descriptor)
    {
        string[] parts = descriptor.Split(";");
        return new CageGroupDescriptorAttributes
        {
            Height = int.Parse(parts[0]),
            Length = int.Parse(parts[1]),
            Width = int.Parse(parts[2]),
            RentedType = Enum.Parse<RentedType>(parts[3]),
            CageType = Enum.Parse<RentingType>(parts[4])
        };
    }

    public override string ToString()
    {
        return CageGroupDescriptor;
    }
}
