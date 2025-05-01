using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    private UIDocument uiDocument;
    public UIDocument uiMainSettings;

    void OnEnable()
    {
        ControllerAudio controllerAudio = this.AddComponent<ControllerAudio>();
        controllerAudio.audioMixer = audioMixer;

        uiDocument = GetComponent<UIDocument>();

        VisualElement container = uiDocument.rootVisualElement.Q<VisualElement>("container");
        new MicrophoneConfig(container[3]);

        uiDocument.rootVisualElement.Q<VisualElement>("buttons")
        .Q<Button>("back-button")
            .RegisterCallback<ClickEvent>(evt => {
                this.SetActive(false);
                uiMainSettings.SetActive(true);
        });
    }

    void OnDisable()
    {
        ControllerAudio controllerAudio = GetComponent<ControllerAudio>();
        Destroy(controllerAudio);
    }
}
