using UnityEngine;

public class ScreenSettingsLoader : IGameSettingsLoader
{
    private readonly int[] fpsOptions = { 30, 60, 120, 144, 240 };

    public void LoadSettings()
    {
        int resolution = Mathf.Clamp(PlayerPrefs.GetInt("resolucion", 0), 0, Screen.resolutions.Length - 1);
        int indexFPS = Mathf.Clamp(PlayerPrefs.GetInt("fps", 0), 0, fpsOptions.Length - 1);

        Resolution[] resolutions = Screen.resolutions;
        Resolution selected = resolutions[resolution];

        Screen.SetResolution(selected.width, selected.height, 
            new ModeScreen().GetFullScreenMode(PlayerPrefs.GetInt("modoPantalla", 0)));

        Application.targetFrameRate = fpsOptions[indexFPS];
        QualitySettings.vSyncCount = 0;
    }
}
