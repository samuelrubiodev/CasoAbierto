using ElevenLabs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMicrophone : MonoBehaviour
{
    private MicrophoneRecorder recorder;
    public IEnumerator RecordAudio()
    {
        // Obtén la referencia al componente MicrophoneRecorder en el mismo GameObject
        recorder = GetComponent<MicrophoneRecorder>();

        if (recorder != null)
        {
            Debug.Log("Iniciando grabación...");
            recorder.StartRecording();

            // Espera unos segundos para grabar audio
            yield return new WaitForSeconds(5);

            Debug.Log("Deteniendo grabación...");
            recorder.StopRecording();

            // Guarda el archivo en la ubicación deseada
            recorder.SaveRecording(Application.persistentDataPath + "/audio.wav");
            Debug.Log("Grabación guardada.");

            APIRequest aPIRequest = GetComponent<APIRequest>();

            llamarApis(aPIRequest);
        }
        else
        {
            Debug.LogError("No se encontró el componente MicrophoneRecorder.");
        }
    }

    private async void llamarApis(APIRequest aPIRequest)
    {
        await aPIRequest.incializarAPITexto();

        APIRequestElevenLabs aPIRequestElevenLabs = GetComponent<APIRequestElevenLabs>();

        aPIRequestElevenLabs.StreamAudio(aPIRequest.promptLLama);
    }
}
