using System;
using System.IO;
using UnityEngine;

public class MicrophoneRecorder : MonoBehaviour
{
    private AudioClip audioClip;
    private bool isRecording = false;
    private string microphoneName;

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneName = PlayerPrefs.GetString("microfono");
        }
        else
        {
            Debug.LogError("No se encontró ningún micrófono.");
        }
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            audioClip = Microphone.Start(microphoneName, true, 10, 44100);
            isRecording = true;
            Debug.Log("Grabación iniciada.");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(microphoneName);
            isRecording = false;
            Debug.Log("Grabación detenida.");

            if (audioClip != null)
            {
                Debug.Log("AudioClip creado con éxito.");
            }
            else
            {
                Debug.LogError("Error al crear el AudioClip.");
            }
        }
    }

    public void SaveRecording(string filePath)
    {
        if (audioClip != null)
        {
            Debug.Log("Iniciando el guardado del archivo...");
            var samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);
            byte[] wavFile = ConvertToWav(samples, audioClip.channels, audioClip.frequency);
            File.WriteAllBytes(filePath, wavFile);
            Debug.Log("Archivo guardado en: " + filePath);
        }
        else
        {
            Debug.LogError("No hay audio grabado para guardar.");
        }
    }

    private byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(stream);

        int headerSize = 44;
        int fileSize = samples.Length * sizeof(short) + headerSize - 8;
        int dataSize = samples.Length * sizeof(short);

        // Escribe el encabezado WAV
        writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
        writer.Write(fileSize);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * sizeof(short));
        writer.Write((short)(channels * sizeof(short)));
        writer.Write((short)16);
        writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
        writer.Write(dataSize);

        // Convierte los datos de audio a formato PCM
        foreach (var sample in samples)
        {
            short intSample = (short)(sample * short.MaxValue);
            writer.Write(intSample);
        }

        writer.Flush();
        return stream.ToArray();
    }
}