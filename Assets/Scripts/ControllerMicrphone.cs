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
        // Obt�n la referencia al componente MicrophoneRecorder en el mismo GameObject
        recorder = GetComponent<MicrophoneRecorder>();

        if (recorder != null)
        {
            Debug.Log("Iniciando grabaci�n...");
            recorder.StartRecording();

            // Espera unos segundos para grabar audio
            yield return new WaitForSeconds(5);

            Debug.Log("Deteniendo grabaci�n...");
            recorder.StopRecording();

            // Guarda el archivo en la ubicaci�n deseada
            recorder.SaveRecording(Application.persistentDataPath + "/audio.wav");
            Debug.Log("Grabaci�n guardada.");

            APIRequest aPIRequest = GetComponent<APIRequest>();

            llamarApis(aPIRequest);
        }
        else
        {
            Debug.LogError("No se encontr� el componente MicrophoneRecorder.");
        }
    }

    private async void llamarApis(APIRequest aPIRequest)
    {
        await aPIRequest.incializarAPITexto();

        APIRequestElevenLabs aPIRequestElevenLabs = GetComponent<APIRequestElevenLabs>();

        aPIRequestElevenLabs.StreamAudio(aPIRequest.promptLLama);
    }
}
