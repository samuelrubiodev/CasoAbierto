using ElevenLabs;
using ElevenLabs.Models;
using ElevenLabs.Voices;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Utilities.Async;

[RequireComponent(typeof(AudioSource))]
public class APIRequestElevenLabs : MonoBehaviour
{
    [SerializeField]
    private ElevenLabsConfiguration configuration;

    [SerializeField]
    private bool debug = true;

    [SerializeField]
    private Voice voice;

    [TextArea(3, 10)]
    [SerializeField]
    private string message;

    [SerializeField]
    private AudioSource audioSource;

    private readonly Queue<AudioClip> streamClipQueue = new();

    private void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public async void StreamAudio(String message)
    {
        this.message = message;

        OnValidate();

        try
        {
            var api = new ElevenLabsClient(configuration)
            {
                EnableDebug = debug
            };

            if (voice == null)
            {
                voice = (await api.VoicesEndpoint.GetVoiceAsync("7ilYbYb99yBZGMUUKSaf"));
            }


            /// 0 - default mode (no latency optimizations)<br/>
            /// 1 - normal latency optimizations (about 50% of possible latency improvement of option 3)<br/>
            /// 2 - strong latency optimizations (about 75% of possible latency improvement of option 3)<br/>
            /// 3 - max latency optimizations<br/>
            int ? optimizeStreamingLatency = 0;

            streamClipQueue.Clear();
            var streamQueueCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            PlayStreamQueue(streamQueueCts.Token);
            var voiceClip = await api.TextToSpeechEndpoint.StreamTextToSpeechAsync(message, this.voice, partialClip =>
            {
                streamClipQueue.Enqueue(partialClip);
            }, model: Model.TurboV2_5, optimizeStreamingLatency: optimizeStreamingLatency, outputFormat: OutputFormat.PCM_24000, cancellationToken: destroyCancellationToken);
            audioSource.clip = voiceClip.AudioClip;
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
}
