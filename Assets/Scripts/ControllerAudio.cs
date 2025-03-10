using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ControllerAudio : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider sliderMusica;
    public Slider sliderVoces;
    public Slider sliderGeneral;

    private void Awake()
    {
        sliderMusica.onValueChanged.AddListener(ControlMusicaVolumen);
        sliderVoces.onValueChanged.AddListener(ControlVocesVolumen);
        sliderGeneral.onValueChanged.AddListener(ControlGeneralVolumen);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cargar();
    }
}
