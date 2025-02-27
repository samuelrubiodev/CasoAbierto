using System.Collections;
using UnityEngine;

public class ControllerMicrophone : MonoBehaviour
{
    private MicrophoneRecorder recorder;
    public IEnumerator RecordAudio()
    {
        recorder = GetComponent<MicrophoneRecorder>();

        if (recorder != null)
        {
            Debug.Log("Iniciando grabación...");
            recorder.StartRecording();

            yield return new WaitForSeconds(5);

            Debug.Log("Deteniendo grabación...");
            recorder.StopRecording();

            recorder.SaveRecording(Application.persistentDataPath + "/audio.wav");
            Debug.Log("Grabación guardada.");

            APIRequest aPIRequest = GetComponent<APIRequest>();

            LlamarApis(aPIRequest);
        }
        else
        {
            Debug.LogError("No se encontró el componente MicrophoneRecorder.");
        }
    }

    private async void LlamarApis(APIRequest aPIRequest)
    {
        await aPIRequest.incializarAPITexto();

        APIRequestElevenLabs aPIRequestElevenLabs = GetComponent<APIRequestElevenLabs>();
        aPIRequestElevenLabs.StreamAudio(aPIRequest.promptLLama);
    }
}
