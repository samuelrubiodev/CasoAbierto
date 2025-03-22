using System.Collections;
using TMPro;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
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
    private bool CajaTexto = false;
    public static bool estaEscribiendo = false;

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

        if (Input.GetKeyDown(KeyCode.B)) {
            TMP_InputField inputField = texto.GetComponentInChildren<TMP_InputField>();
            
            bool inputFieldTieneFocus = inputField != null && inputField.isFocused;
            if (!inputFieldTieneFocus) {
                if (!CajaTexto) {
                    Mostrar();
                } else {
                    Ocultar();
                }
            }
        } else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            texto.SetActive(false);
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

    private void Mostrar() {
        texto.SetActive(true);
        FirstPersonController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        estaEscribiendo = true;
        CajaTexto = true;
    }

    private void Ocultar() {
        texto.SetActive(false);
        FirstPersonController.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        estaEscribiendo = false;
        CajaTexto = false;
    }

    private IEnumerator StartRecordingProcess(ControllerMicrophone controllerMicrophone)
    {
        isProcessing = true;
        yield return StartCoroutine(controllerMicrophone.RecordAudio());
        isRecordingStarted = false;
        isProcessing = false;
    }
}
