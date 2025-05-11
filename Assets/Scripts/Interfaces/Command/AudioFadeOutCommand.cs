using System.Collections;
using UnityEngine;

public class AudioFadeOutCommand : ICommandEnumerator
{
    private AudioSource audioSource;

    public AudioFadeOutCommand(AudioSource audioSource)
    {
        this.audioSource = audioSource;
    }
    public IEnumerator Execute()
    {
        float time = 0f;
        float startVol = audioSource.volume;
        float duration = 1f;
        float targetVolume = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol,targetVolume,time / duration);
            yield return null;
        }
    }
}