using Unity.VisualScripting;
using UnityEngine;

public class CalmMusic : MusicEvent
{
    public CalmMusic(AudioClip audioClip, int priority, bool shouldLoop = true) : base(audioClip, priority, shouldLoop) { }

    public override bool CheckCondition()
    {
        return CoundownTimer.countdownInternal>= 81.6f;
    }

    public override float GetDuration()
    {
        return (float)audioClip.samples / audioClip.frequency;

    }
}