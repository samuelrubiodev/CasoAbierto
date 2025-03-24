using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
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
}

public class SubtitleManager
{
    public string path { get; set; }
    public string texto { get; set; }

    public SubtitleManager(string path)
    { 
        this.path = path;
    }

    public SubtitleList ReadJSON()
    {
        string json = System.IO.File.ReadAllText(path);
        SubtitleList subtitulos = JsonUtility.FromJson<SubtitleList>(json);
        return subtitulos;
    }
}
