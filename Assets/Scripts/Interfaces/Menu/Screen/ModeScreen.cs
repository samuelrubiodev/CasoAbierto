using UnityEngine;
using UnityEngine.UIElements;

public class ModeScreen : ISetting<int>
{
    private VisualElement value { get; set; }
    public ModeScreen(VisualElement value)
    {
        this.value = value;
        GetValue();
    }

    public ModeScreen()
    {
        this.value = null;
    }

    public FullScreenMode GetFullScreenMode(int index)
    {
        return index switch
        {
            0 => FullScreenMode.ExclusiveFullScreen,
            1 => FullScreenMode.Windowed,
            _ => FullScreenMode.Windowed
        };
    }

    public void GetValue()
    {
        if (value.name == "mode-screen")
        {
            DropdownField list = value.Q<DropdownField>();

            if (PlayerPrefs.HasKey("modoPantalla"))
            {
                int modeScreen = PlayerPrefs.GetInt("modoPantalla");
                list.index = modeScreen;
            }
        }
    }

    public void ApplyModeScreenSettings()
    {
        DropdownField list = value.Q<DropdownField>();
        SetValue(list.index);
    }

    public void SetValue(int newValue)
    {
        PlayerPrefs.SetInt("modoPantalla", newValue);
        PlayerPrefs.Save();
    }
}