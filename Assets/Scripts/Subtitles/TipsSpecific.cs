using UnityEngine;

public class TipsSpecific : ISubtitles
{
    private readonly string path = "/Resources/Subtitles/TipsSpecific.json";
    public SubtitleList ReturnSubtitleList()
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SubtitleList>(json);
    }
}