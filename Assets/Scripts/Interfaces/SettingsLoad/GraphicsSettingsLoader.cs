using UnityEngine;

public class GraphicsSettingsLoader : IGameSettingsLoader
{
    public void LoadSettings()
    {
        int calidad = PlayerPrefs.GetInt("calidadGraficos", 0);
        QualitySettings.SetQualityLevel(calidad, true);
    }
}
