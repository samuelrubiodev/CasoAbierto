using ElevenLabs;
using ElevenLabs.Models;
using ElevenLabs.Voices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.Async;

[RequireComponent(typeof(AudioSource))]
public class APIRequestElevenLabs : MonoBehaviour
{
    private static string AUDIO_ID_HOMBRE = "BPoDAH7n4gFrnGY27Jkj";
    private static string AUDIO_ID_MUJER = "UOIqAnmS11Reiei1Ytkc";

    [SerializeField]
    private bool debug = true;

    private Voice voice;
    private Voice voiceMan;
    private Voice voiceWoman;
    private string message;

    [SerializeField]
    private AudioSource audioSource;

    private readonly Queue<AudioClip> streamClipQueue = new();
    private ElevenLabsClient api;
   
    private void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    async void Start()
    {
        api = new ElevenLabsClient(new ElevenLabsAuthentication(ApiKey.API_KEY_ELEVENLABS))
        {
            EnableDebug = debug
        };

        voiceMan = await api.VoicesEndpoint.GetVoiceAsync(AUDIO_ID_HOMBRE);
        voiceWoman = await api.VoicesEndpoint.GetVoiceAsync(AUDIO_ID_MUJER);
    }

    public async void StreamAudio(string message, bool isMan)
    {
        this.message = message;

        OnValidate();

        try
        {
            voice = isMan 
                ?  voiceMan
                : voiceWoman;

            /// 0 - default mode (no latency optimizations)<br/>
            /// 1 - normal latency optimizations (about 50% of possible latency improvement of option 3)<br/>
            /// 2 - strong latency optimizations (about 75% of possible latency improvement of option 3)<br/>
            /// 3 - max latency optimizations<br/>
            int? optimizeStreamingLatency = 1;

            streamClipQueue.Clear();
            var streamQueueCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            var playTask = Task.Run(() => PlayStreamQueue(streamQueueCts.Token));

            var streamTask = Task.Run(async () => {
                var voiceClip = await api.TextToSpeechEndpoint.StreamTextToSpeechAsync(message, this.voice, partialClip =>
                {
                    streamClipQueue.Enqueue(partialClip);
                }, model: new Model("eleven_flash_v2_5"), optimizeStreamingLatency: optimizeStreamingLatency, outputFormat: OutputFormat.PCM_24000, cancellationToken: destroyCancellationToken);
                return voiceClip;
            });
            
            var voiceClip = await streamTask;
            //audioSource.clip = voiceClip.AudioClip;
            await new WaitUntil(() => streamClipQueue.Count == 0 && !audioSource.isPlaying);
            streamQueueCts.Cancel();

            if (debug)
            {
                Debug.Log($"Full clip: {voiceClip.Id}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private async void PlayStreamQueue(CancellationToken cancellationToken)
    {
        try
        {
            await new WaitUntil(() => streamClipQueue.Count > 0);
            var endOfFrame = new WaitForEndOfFrame();

            do
            {
                if (!audioSource.isPlaying &&
                    streamClipQueue.TryDequeue(out var clip))
                {
                    Debug.Log($"playing partial clip: {clip.name}");
                    audioSource.PlayOneShot(clip);
                }

                await endOfFrame;
            } while (!cancellationToken.IsCancellationRequested);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public AudioSource GetAudioSource()
    {
        return audioSource;
    }
}