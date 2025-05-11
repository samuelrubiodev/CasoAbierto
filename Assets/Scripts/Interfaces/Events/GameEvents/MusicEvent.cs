using UnityEngine;

public interface IMusic 
{
    bool CheckCondition();
}
public abstract class MusicEvent : GameEvent, IMusic
{
    public AudioClip audioClip;
    public int priority = 0;
    public bool shouldLoop = false;
    public MusicEvent(AudioClip audioClip, int priority = 0, bool shouldLoop = false)
    {
        this.audioClip = audioClip;
        this.priority = priority;
        this.shouldLoop = shouldLoop;
    }

    public abstract bool CheckCondition();
    public abstract float GetDuration();
}