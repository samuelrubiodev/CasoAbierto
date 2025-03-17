using System.Collections;
using TMPro;
using UnityEngine;

public class ControllerGame : MonoBehaviour
{
    private bool isRecordingStarted = false;
    private bool isProcessing = false;
    public bool isGameInProgress { get; set; }

    private ControllerMicrophone controllerMicrophone;  
    public FirstPersonController FirstPersonController;
    public GameObject texto;
    private CoundownTimer coundownTimer;
    private bool seHaIniciadoContador = false;

    void Start()
    {
        coundownTimer = GetComponent<CoundownTimer>();
        controllerMicrophone = GetComponent<ControllerMicrophone>();
    }

    void Update()
    {
        if (isGameInProgress && !seHaIniciadoContador)
        {
            seHaIniciadoContador = true;
            coundownTimer.EmpezarContador();
        }

        if (coundownTimer.countdownOver)
        {
            isGameInProgress = false;
        }

        if (Input.GetKey(KeyCode.B)) {
            FirstPersonController.enabled = false;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            FirstPersonController.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(StartRecordingProcess(controllerMicrophone));
        }

        if (Input.GetKey(KeyCode.E) && isGameInProgress && !isProcessing)
        {
            if (!isRecordingStarted)
            {
                controllerMicrophone = GetComponent<ControllerMicrophone>();
                if (controllerMicrophone != null)
                {
                    StartCoroutine(StartRecordingProcess(controllerMicrophone));
                    isRecordingStarted = true;
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
        texto.transform.GetComponent<TMP_InputField>().text = "";
    }
}
