using System.Collections.Generic;

public class Config
{
    private static Dictionary<string, string> dict = new Dictionary<string, string>();
    public static void Set(string data)
    {
        dict.Clear();
        string[] lines = data.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] args = lines[i].Replace(" ", "").Trim().Split('=');
            if (args.Length > 1) dict.Add(args[0], args[1]);
        }
    }

    public static string Get(string key)
    {
        if (dict.ContainsKey(key))
            return dict[key];
        return "";
    }
}
