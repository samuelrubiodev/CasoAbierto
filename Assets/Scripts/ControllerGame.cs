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

        /*
         * Inicializacion inicializacion = new("Samuel");

        // Prueba de creación de bases de datos SQLite y Redis
        await inicializacion.crearBaseDatosSQLite("OpenRouter", "[API_REMOVED]");
        await inicializacion.crearBaseDatosSQLite("ElevenLabs", "[API_REMOVED]");
        await inicializacion.crearBaseDatosRedis("[IP_REMOVED]", "6379", "", "[PASSWORD_REMOVED]");
         * 
         * 
         * 
         * 
        */


    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && isGameInProgress && !isProcessing)
        {
            if (!isRecordingStarted)
            {
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
