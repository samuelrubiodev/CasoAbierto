using UnityEngine;
using UnityEngine.UIElements;

public class MicrophoneConfig : ISetting<string>
{
    public VisualElement value { get; set; }

    public MicrophoneConfig(VisualElement value)
    {
        this.value = value;
        GetValue();

        DropdownField list = value.Q<DropdownField>();
        list.RegisterValueChangedCallback(evt =>
        {
            SetValue(evt.newValue);
        });
    }

    public void GetValue()
    {
        if (value.name == "microphone")
        {
            DropdownField list = value.Q<DropdownField>();
            list.choices.Clear();

            string[] microphones = Microphone.devices;

            for (int i = 0; i < microphones.Length; i++)
            {
                list.choices.Add(microphones[i]);
            }

            list.choices.Add("Solo texto");

            for (int i = 0; i < list.choices.Count; i++)
            {
                string microphoneSelected = PlayerPrefs.GetString("microfono");
                if (list.choices[i] == microphoneSelected)
                {
                    list.index = i;
                    break;
                }
            }
        }
    }

    public void SetValue(string newValue)
    {
        PlayerPrefs.SetString("microfono", newValue);
        PlayerPrefs.Save();
    }
}
