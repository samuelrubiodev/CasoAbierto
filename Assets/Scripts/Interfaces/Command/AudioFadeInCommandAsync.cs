using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AudioFadeInCommandAsync : ICommand
{
    private AudioSource audioSource;

    public AudioFadeInCommandAsync(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }


    public async Task Execute()
    {
        audioSource.volume = 0f;
        float time = 0f;
        float startVol = audioSource.volume;
        float duration = 1f;
        float targetVolume = 1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol,targetVolume,time / duration);
            await Task.Yield();
        }        
    }
}