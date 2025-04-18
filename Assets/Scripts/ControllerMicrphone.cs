using System.Collections;
using TMPro;
using UnityEngine;

public class ControllerMicrophone : MonoBehaviour
{
    private MicrophoneRecorder recorder;
    public GameObject elementoTexto;
    public IEnumerator RecordAudio()
    {
        recorder = GetComponent<MicrophoneRecorder>();

        if (recorder != null && recorder.TieneMicrofono())
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
        else if (!recorder.TieneMicrofono())
        {
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
        APIRequestElevenLabs aPIRequestElevenLabs = GetComponent<APIRequestElevenLabs>();
        await aPIRequest.RequestAPI(aPIRequestElevenLabs, elementoTexto.GetComponent<TMP_InputField>().text);
    }
}
