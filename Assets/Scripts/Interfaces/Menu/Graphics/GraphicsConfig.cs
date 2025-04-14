using UnityEngine;
using UnityEngine.UIElements;

public class GraphicsConfig : ISetting<int>
{
    private VisualElement value { get; set; }

    public GraphicsConfig(VisualElement value)
    {
        this.value = value;
        GetValue();
    }
    public void GetValue()
    {
        if (value.name == "quality-graphics")
        {
            DropdownField list = value.Q<DropdownField>();

            if (PlayerPrefs.HasKey("calidadGraficos"))
            {
                int qualityGrpaphics = PlayerPrefs.GetInt("calidadGraficos");
                list.index = qualityGrpaphics;
            }
        }
    }

    public void ApplyQualitySettings()
    {
        DropdownField list = value.Q<DropdownField>();
        PlayerPrefs.SetInt("calidadGraficos", list.index);
        PlayerPrefs.Save();
        SetValue(list.index);
    }

    public void SetValue(int newValue)
    {
        QualitySettings.SetQualityLevel(newValue, true);
    }
}