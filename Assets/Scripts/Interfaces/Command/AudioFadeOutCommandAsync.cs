using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AudioFadeOutCommandAsync : ICommand
{
    private AudioSource audioSource;

    public AudioFadeOutCommandAsync(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }
    public async Task Execute()
    {
        audioSource.volume = 0f;
        float time = 0f;
        float startVol = audioSource.volume;
        float duration = 1f;
        float targetVolume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol,targetVolume,time / duration);
            await Task.Yield();
        }
    }
}