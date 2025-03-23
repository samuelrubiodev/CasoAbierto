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

    public async static void AddSubtitleToGUI(string[] strings, TMP_Text textoSubtitulos) 
    {
        StringBuilder buffer = new();
        
        for (int i = 0; i < strings.Length; i++)
        {
            buffer.Append(strings[i] + " ");
            if (i % 5 == 0)
            {
                textoSubtitulos.text = buffer.ToString();
                buffer.Clear();
                await Task.Delay(2000);
            }
            else if (i == strings.Length - 1)
            {
                textoSubtitulos.text = buffer.ToString();
                await Task.Delay(2000);
            }
        }
    }

}
