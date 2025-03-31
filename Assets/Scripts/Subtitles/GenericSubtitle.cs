using UnityEngine;

public class GenericSubtitles : ISubtitles
{
    private readonly string path;
    public GenericSubtitles(string jsonPath)
    {
        this.path = jsonPath;
    }

    public SubtitleList ReturnSubtitleList()
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SubtitleList>(json);
    }
}