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
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        return JsonUtility.FromJson<SubtitleList>(textAsset.text);
    }
}