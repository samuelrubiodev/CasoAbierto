using UnityEngine;

public class TipsResearch : ISubtitles
{
    private readonly string  path = Application.dataPath + "/Resources/Subtitles/TipsResearch.json";
    public SubtitleList ReturnSubtitleList()
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SubtitleList>(json);
    }
}
