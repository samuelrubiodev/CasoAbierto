using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Subtitle
{
    public int id;
    public String[] text;
}

[Serializable]
public class SubtitleList
{
    public List<Subtitle> subtitles;

    public string[] ToArray()
    {
        string[] array = new string[subtitles.Count];
        for (int i = 0; i < subtitles.Count; i++)
        {
            for (int j = 0; j < subtitles[i].text.Length; j++)
            {
                array[i] = subtitles[i].text[j];
            }
        }
        return array;
    }
}

public class SubtitleManager : IJsonUtilities<SubtitleList>
{
    public string path { get; set; }
    public string texto { get; set; }

    public SubtitleManager(string path)
    { 
        this.path = path;
    }

    public SubtitleList ReadJSON()
    {
        string json = Resources.Load<TextAsset>(path).text;
        SubtitleList subtitulos = JsonUtility.FromJson<SubtitleList>(json);
        return subtitulos;
    }
}
