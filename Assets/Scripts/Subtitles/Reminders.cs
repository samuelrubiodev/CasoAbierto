using UnityEngine;

public class Reminders : ISubtitles
{
    private readonly string path = "/Resources/Subtitles/Reminders.json";
    public SubtitleList ReturnSubtitleList()
    {
        string json = System.IO.File.ReadAllText(path);
        return JsonUtility.FromJson<SubtitleList>(json);
    }
}