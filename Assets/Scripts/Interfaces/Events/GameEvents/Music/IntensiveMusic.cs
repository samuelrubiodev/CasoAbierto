using UnityEngine;

public class IntensiveMusic : MusicEvent
{
    public IntensiveMusic(AudioClip audioClip, int priority, bool shoudlLoop = false) : base(audioClip,priority, shoudlLoop) { }

    public override bool CheckCondition()
    { 
        return CoundownTimer.countdownInternal <= 81.6f;
    }

    public override float GetDuration()
    {
        return 81.6f;
    }
}