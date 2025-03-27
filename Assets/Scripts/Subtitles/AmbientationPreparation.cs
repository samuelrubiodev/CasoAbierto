using UnityEngine;

public class AmbientationPreparation : ISubtitles
{
    private readonly string path = "/Resources/Subtitles/AmbientationPreparation.json";
    public SubtitleList ReturnSubtitleList()
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SubtitleList>(json);
    }
}