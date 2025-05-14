using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip calmMusic;
    [SerializeField] private AudioClip intensiveMusic;
    [SerializeField] private AudioClip bossMusic;
    public static AudioSource audioSource;
    public static float seconds { get; private set; } = 300f;
    private List<MusicEvent> musicEvents = new ();
    private MusicEvent currentMusicEvent;
    private Coroutine currentMusicCoroutine;

    void Awake()
    {
        musicEvents = new List<MusicEvent>() {
            new CalmMusic(calmMusic,2,true),
            new IntensiveMusic(intensiveMusic,1,false),
        };

        audioSource = GetComponent<AudioSource>();
    }

    void PlayMusic(MusicEvent musicEvent)
    {
        if (currentMusicCoroutine != null)
        {
            StopCoroutine(currentMusicCoroutine);
        }

        currentMusicCoroutine = StartCoroutine(PlayMusicCoroutine(musicEvent));
    }

    IEnumerator PlayMusicCoroutine(MusicEvent musicEvent)
    {
        if (audioSource.isPlaying)
        {
            yield return new AudioFadeOutCommand(audioSource).Execute();
            audioSource.Stop();
        }

        audioSource.clip = musicEvent.audioClip;
        audioSource.loop = musicEvent.shouldLoop;
        audioSource.Play();
        yield return new AudioFadeInCommand(audioSource).Execute();

    }


    void Update()
    {
        seconds -= Time.deltaTime;

        MusicEvent bestMusicEvent = null;

        foreach (var musicEvent in musicEvents)
        {
            if (musicEvent.CheckCondition())
            {
                if (musicEvent.priority > (bestMusicEvent?.priority ?? 0))
                {
                    bestMusicEvent = musicEvent;
                }
            }
        }

        bestMusicEvent ??= musicEvents[0];


        if (currentMusicEvent == null || currentMusicEvent != bestMusicEvent)
        {
            currentMusicEvent = bestMusicEvent;
            PlayMusic(bestMusicEvent);
        } else
        {
            return;
        }
    }
}