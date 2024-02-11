using System;
using System.IO;

namespace camagru.Utilities;

public static class DotEnv
{
    public static void Load(string filepath)
    {
        if (!File.Exists(filepath))
            return;
        foreach (var line in File.ReadAllLines(filepath))
        {
            var parts = line.Split(
                "=",
                StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                continue;
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}