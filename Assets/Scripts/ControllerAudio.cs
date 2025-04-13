using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class ControllerAudio : MonoBehaviour
{
    public AudioMixer audioMixer;
    private Slider sliderMusica;
    private Slider sliderVoces;
    private Slider sliderGeneral;
    public UIDocument uiAudioSettings;

    void Awake()
    {
        VisualElement container = uiAudioSettings.rootVisualElement.Q<VisualElement>("container");
        this.sliderGeneral = container[0].Q<Slider>("slider-general");
        this.sliderMusica = container[1].Q<Slider>("slider-music");
        this.sliderVoces = container[2].Q<Slider>("slider-voices");

        sliderMusica.RegisterCallback<ChangeEvent<float>>(evt => ControlMusicaVolumen(evt.newValue));
        sliderVoces.RegisterCallback<ChangeEvent<float>>(evt => ControlVocesVolumen(evt.newValue));
        sliderGeneral.RegisterCallback<ChangeEvent<float>>(evt => ControlGeneralVolumen(evt.newValue));
    }

    private void ControlMusicaVolumen(float volumen)
    {
        audioMixer.SetFloat("VolumenMusica", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("volumenMusica", volumen);
    }

    private void ControlVocesVolumen(float volumen)
    {
        audioMixer.SetFloat("VolumenVoces", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("volumenVoces", volumen);
    }

    private void ControlGeneralVolumen(float volumen)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("volumenGeneral", volumen);
    }

    public void Cargar()
    {
        sliderMusica.value = PlayerPrefs.GetFloat("volumenMusica", 0.75f);
        sliderVoces.value = PlayerPrefs.GetFloat("volumenVoces", 0.75f);
        sliderGeneral.value = PlayerPrefs.GetFloat("volumenGeneral", 0.75f);

        ControlMusicaVolumen(sliderMusica.value);
        ControlVocesVolumen(sliderVoces.value);
        ControlGeneralVolumen(sliderGeneral.value);
    }

    void Start()
    {
        Cargar();
    }
}
