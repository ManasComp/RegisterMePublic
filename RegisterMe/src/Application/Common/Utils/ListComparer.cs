namespace RegisterMe.Application.Common.Utils;

public class ListComparer<T> : IEqualityComparer<List<T>>
{
    public bool Equals(List<T>? x, List<T>? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return x.Count == y.Count && x.TrueForAll(y.Contains);
    }

    public int GetHashCode(List<T> obj)
    {
        return obj.Aggregate(0, (hash, item) =>
        {
            if (item != null)
            {
                return hash ^ item.GetHashCode();
            }

            return 0;
        });
    }
}
