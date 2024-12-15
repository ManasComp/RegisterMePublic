namespace RegisterMe.Application.Exhibitions.Dtos;

public record GroupId : IComparable<GroupId>
{
    public GroupId(string input)
    {
        string numericPartString = "";
        string alphaPartString = "";
        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                numericPartString += c;
            }
            else
            {
                alphaPartString += c;
            }
        }

        AlphaPart = alphaPartString;
        NumericPart = int.Parse(numericPartString);
    }

    private int NumericPart { get; }
    private string AlphaPart { get; }

    public int CompareTo(GroupId? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }

        int numericPartComparison = NumericPart.CompareTo(other.NumericPart);
        return numericPartComparison != 0
            ? numericPartComparison
            : string.Compare(AlphaPart, other.AlphaPart, StringComparison.Ordinal);
    }
}
