using UnityEngine;
using UnityEngine.UIElements;

public class FPSConfig : ISetting<int>
{
    private VisualElement value { get; set; }

    public FPSConfig(VisualElement value)
    {
        this.value = value;
        GetValue();
    }
    public void GetValue()
    {
        if (value.name == "limit-fps")
        {
            DropdownField list = value.Q<DropdownField>();

            if (PlayerPrefs.HasKey("fps"))
            {
                int fps = PlayerPrefs.GetInt("fps");
                list.index = fps;
            }
        }
    }

    public void ApplyFPSSetting()
    {
        DropdownField list = value.Q<DropdownField>();
        PlayerPrefs.SetInt("fps", list.index);
        PlayerPrefs.Save();
        SetValue(ScreenSettingsLoader.fpsOptions[list.index]);
    }

    public void SetValue(int newValue)
    {
        Application.targetFrameRate = newValue;
    }
}