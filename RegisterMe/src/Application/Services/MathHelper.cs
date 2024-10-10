namespace RegisterMe.Application.Services;

public static class MathHelper
{
    public static List<List<T>> GetCombinationsOfGivenList<T>(List<T> arr)
    {
        List<List<T>> result = [];
        GetCombinations(arr, 0, [], result);
        result.Sort((a, b) => a.Count.CompareTo(b.Count));
        return result;
    }

    private static void GetCombinations<T>(List<T> arr, int index, List<T> current, List<List<T>> result)
    {
        while (true)
        {
            if (index >= arr.Count)
            {
                if (current.Count > 0)
                {
                    result.Add([..current]);
                }

                return;
            }

            current.Add(arr[index]);
            GetCombinations(arr, index + 1, current, result);

            current.RemoveAt(current.Count - 1);
            index += 1;
        }
    }
}
