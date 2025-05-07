using System.Collections;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;

public class ControllerMicrophone : MonoBehaviour
{
    private MicrophoneRecorder recorder;
    public GameObject elementoTexto;
    public IEnumerator RecordAudio(string prompt)
    {
        recorder = GetComponent<MicrophoneRecorder>();

        yield return new WaitForSeconds(1);

        if (recorder != null && recorder.TieneMicrofono())
        {
            recorder.StartRecording();
            while (!Input.GetKeyDown(KeyCode.E))
            {
                yield return null;
            }

            int samplesRecorded = Microphone.GetPosition(recorder.microphoneName);
            recorder.StopRecording();
            recorder.SaveRecording(Application.persistentDataPath + "/audio.wav", samplesRecorded);

            APIRequest aPIRequest = GetComponent<APIRequest>();

            LlamarApis(aPIRequest, prompt);
        }
        else if (!recorder.TieneMicrofono())
        {
            APIRequest aPIRequest = GetComponent<APIRequest>();
            LlamarApis(aPIRequest, prompt);
        }
        else
        {
            Debug.LogError("No se encontró el componente MicrophoneRecorder.");
        }
    }

    private async void LlamarApis(APIRequest aPIRequest, string prompt)
    {
        Debug.Log(ApiKey.API_KEY_ELEVENLABS);
        APIRequestElevenLabs aPIRequestElevenLabs = GetComponent<APIRequestElevenLabs>();
        APICreditsManager aPICreditsManager = GetComponent<APICreditsManager>();

        await aPIRequest.RequestAPI(aPIRequestElevenLabs, prompt);
        aPICreditsManager.isGameStarted = true;
    }
}
