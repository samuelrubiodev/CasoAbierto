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
    private bool CajaTexto = false;
    public static bool estaEscribiendo = false;
    public GameObject personajes;
    public GameObject PrefabCharacter;

    void Start()
    {
        coundownTimer = GetComponent<CoundownTimer>();
        controllerMicrophone = GetComponent<ControllerMicrophone>();
    }

    void Update()
    {
        CheckGenre();

        if (isGameInProgress && !seHaIniciadoContador)
        {
            seHaIniciadoContador = true;
            coundownTimer.EmpezarContador();
        }

        if (PlayerPrefs.GetString("microfono") == "Texto") 
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                TMP_InputField inputField = texto.GetComponentInChildren<TMP_InputField>();
                bool inputFieldTieneFocus = inputField != null && inputField.isFocused;
                if (!inputFieldTieneFocus) {
                    if (!CajaTexto) {
                        Show();
                    } else {
                        Hide();
                    }
                }
            } else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) && isGameInProgress)
            {
                Hide();
                StartCoroutine(StartRecordingProcess(controllerMicrophone));
            }
        } else if (Input.GetKey(KeyCode.E) && isGameInProgress && !isProcessing) {
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

    private void Show() {
        texto.SetActive(true);
        FirstPersonController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        estaEscribiendo = true;
        CajaTexto = true;
    }

    private void Hide() {
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

    private void DeactivateCharacter() 
    {
        personajes.transform.GetChild(0).gameObject.SetActive(false);
        personajes.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void ShowGUICharacter() 
    {
        PrefabCharacter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = MenuPersonajes.personajeSeleccionado.nombre;
        PrefabCharacter.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = MenuPersonajes.personajeSeleccionado.estado;
        PrefabCharacter.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = MenuPersonajes.personajeSeleccionado.estadoEmocional;
        PrefabCharacter.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = MenuPersonajes.personajeSeleccionado.sexo;

        PrefabCharacter.SetActive(true);
    }

    private void CheckGenre() {
        if (MenuPersonajes.personajeSeleccionado.sexo == "Masculino" && MenuPersonajes.hasChangedCharacter)
        {
            DeactivateCharacter();
            personajes.transform.GetChild(0).gameObject.SetActive(true);
            MenuPersonajes.hasChangedCharacter = false;
            ShowGUICharacter();
        }
        else if (MenuPersonajes.personajeSeleccionado.sexo == "Femenino" && MenuPersonajes.hasChangedCharacter)
        {
            DeactivateCharacter();
            personajes.transform.GetChild(1).gameObject.SetActive(true);
            MenuPersonajes.hasChangedCharacter = false;
            ShowGUICharacter();
        }
    }
}
