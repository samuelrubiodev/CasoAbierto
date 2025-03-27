using UnityEngine;

public class ShortText : ISubtitles
{
    private readonly string path = "/Resources/Subtitles/ShortText.json";
    public SubtitleList ReturnSubtitleList()
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SubtitleList>(json);
    }
}