using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsLoader : IGameSettingsLoader
{
    private AudioMixer _audioMixer;

    public AudioSettingsLoader(AudioMixer mixer)
    {
        _audioMixer = mixer;
    }

    public void LoadSettings()
    {
        float volumenMusica = PlayerPrefs.GetFloat("volumenMusica", 0.75f);
        float volumenVoces = PlayerPrefs.GetFloat("volumenVoces", 0.75f);
        float volumenGeneral = PlayerPrefs.GetFloat("volumenGeneral", 0.75f);

        _audioMixer.SetFloat("VolumenMusica", Mathf.Log10(volumenMusica) * 20);
        _audioMixer.SetFloat("VolumenVoces", Mathf.Log10(volumenVoces) * 20);
        _audioMixer.SetFloat("Master", Mathf.Log10(volumenGeneral) * 20);
    }
}