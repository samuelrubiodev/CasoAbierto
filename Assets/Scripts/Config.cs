using UnityEngine;

public class Config
{
    private static TextAsset config;

    public Config(string folder) {
        config = Resources.Load<TextAsset>(folder);
    }

    public string GetKey(string key)
    {
        string value = "";
        if (config != null)
        {
            foreach (string line in config.text.Split('\n'))
            {
                if (line.Contains(key+"=")) value = line.Split('=')[1].Trim();
            }
        }
        return value;
    }
}
