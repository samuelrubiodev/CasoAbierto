using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGame : MonoBehaviour
{
    private bool isRecordingStarted = false;
    private bool isProcessing = false;
    public bool isGameInProgress { get; set; }

    private ControllerMicrophone controllerMicrophone;

    void Start()
    {
        controllerMicrophone = GetComponent<ControllerMicrophone>(); 
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && isGameInProgress && !isProcessing)
        {
            if (!isRecordingStarted)
            {
                Debug.Log("Dentro del IF");
                controllerMicrophone = this.GetComponent<ControllerMicrophone>();
                if (controllerMicrophone != null)
                {
                    StartCoroutine(StartRecordingProcess(controllerMicrophone));
                    isRecordingStarted = true;
                }
                else
                {
                    Debug.LogError("ControllerMicrophone no encontrado");
                }
            }
        }
    }

    private IEnumerator StartRecordingProcess(ControllerMicrophone controllerMicrophone)
    {
        isProcessing = true;
        yield return StartCoroutine(controllerMicrophone.RecordAudio());
        isRecordingStarted = false;
        isProcessing = false;
    }
}
