using UnityEngine;
using UnityEngine.UIElements;

public class ScreenConfig : ISetting<int>
{
    private VisualElement value { get; set; }
    private UIDocument uiDocument;
    private ModeScreen modeScreen;

    public ScreenConfig(VisualElement value, UIDocument uiDocument, ModeScreen modeScreen)
    {
        this.value = value;
        this.uiDocument = uiDocument;
        this.modeScreen = modeScreen;
        GetValue();
    }

    public void GetValue()
    {
        if (value.name == "resolution")
        {
            DropdownField list = value.Q<DropdownField>();
            Resolution[] resolutions = Screen.resolutions;
            list.choices.Clear();

            for (int i = 0; i < resolutions.Length; i++)
            {
                list.choices.Add(resolutions[i].width + "x" + resolutions[i].height);
            }

            if (PlayerPrefs.HasKey("resolucion"))
            {
                int resolution = PlayerPrefs.GetInt("resolucion");
                list.index = resolution;
            }
        }
    }

    public void ApplyScreenSettings()
    {
        DropdownField list = value.Q<DropdownField>();
        PlayerPrefs.SetInt("resolucion", list.index);
        PlayerPrefs.Save();
        SetValue(list.index);
    }

    public void SetValue(int newValue)
    {
        Resolution[] resolutions = Screen.resolutions;
        int width = resolutions[newValue].width;
        int height = resolutions[newValue].height;

        VisualElement containerModeScreen = uiDocument.rootVisualElement.Q<VisualElement>("mode-screen");

        Screen.SetResolution(width, height, modeScreen.GetFullScreenMode(containerModeScreen.Q<DropdownField>().index));
    }
}