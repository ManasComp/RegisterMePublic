namespace RegisterMe.Infrastructure.Services;

public static class DotEnv
{
    private static readonly char[] Separator = ['='];

    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        foreach (string? line in File.ReadAllLines(filePath))
        {
            string[] parts = line.Split(Separator, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                continue;
            }

            if (parts[0].StartsWith("#"))
            {
                continue;
            }

            if (parts[0].StartsWith("\""))
            {
                parts[0] = parts[0].Substring(1, parts[0].Length - 2);
            }

            if (parts[1].StartsWith("\""))
            {
                parts[1] = parts[1].Substring(1, parts[1].Length - 2);
            }

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}
