using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerGame : MonoBehaviour
{
    private static ControllerGame instance;
    private bool isRecordingStarted = false;
    private bool isProcessing = false;
    public bool isGameInProgress { get; set; }
    private ControllerMicrophone controllerMicrophone;  
    public FirstPersonController FirstPersonController;
    public GameObject texto;
    public CoundownTimer coundownTimer;
    private bool seHaIniciadoContador = false;
    public static bool estaEscribiendo = false;
    public GameObject personajes;
    public GameObject PrefabCharacter;
    public MessageInputText messageInputText;

    private ControllerGame() 
    {
    }

    public static ControllerGame GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        coundownTimer = GetComponent<CoundownTimer>();

        seHaIniciadoContador = true;
        coundownTimer.EmpezarContador();
    }

    void Start()
    {
        controllerMicrophone = GetComponent<ControllerMicrophone>();
        messageInputText.OnPromptSubmitted += GetPrompt;
        instance = this;
    }

    private void GetPrompt(string prompt)
    {
        StartCoroutine(StartRecordingProcess(controllerMicrophone, prompt));
    }

    void Update()
    {
        CheckGenre();

        if (coundownTimer.countdownOver) {
            FinalSceneManager.isUserWin = false;
            SceneManager.LoadScene("FinalScene");
        }

        if (PlayerPrefs.GetString("microfono") != "Solo texto")
        {
            if (Input.GetKeyDown(KeyCode.H) && isGameInProgress && !isProcessing) 
            {
                if (!isRecordingStarted)
                {
                    controllerMicrophone = GetComponent<ControllerMicrophone>();
                    if (controllerMicrophone != null)
                    {
                        StartCoroutine(StartRecordingProcess(controllerMicrophone,""));
                        isRecordingStarted = true;
                    }
                }
            }
        }
    }
    
    private IEnumerator StartRecordingProcess(ControllerMicrophone controllerMicrophone, string prompt)
    {
        isProcessing = true;
        yield return StartCoroutine(controllerMicrophone.RecordAudio(prompt));
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
        PrefabCharacter.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = SelectionCharacters.selectedCharacter.nombre;

        GameObject statusGameObject = PrefabCharacter.transform.GetChild(1).gameObject;
        GameObject emotionalStatusObject = PrefabCharacter.transform.GetChild(2).gameObject;
        GameObject genreObject = PrefabCharacter.transform.GetChild(3).gameObject;

        statusGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = SelectionCharacters.selectedCharacter.estado;
        emotionalStatusObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = SelectionCharacters.selectedCharacter.estadoEmocional;
        genreObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = SelectionCharacters.selectedCharacter.sexo;

        PrefabCharacter.SetActive(true);
    }

    private void CheckGenre() {
        if (SelectionCharacters.selectedCharacter.sexo.ToLower() == "masculino" && SelectionCharacters.hasChangedCharacter)
        {
            DeactivateCharacter();
            personajes.transform.GetChild(0).gameObject.SetActive(true);
            SelectionCharacters.hasChangedCharacter = false;
            ShowGUICharacter();
        }
        else if (SelectionCharacters.selectedCharacter.sexo.ToLower() == "femenino" && SelectionCharacters.hasChangedCharacter)
        {
            DeactivateCharacter();
            personajes.transform.GetChild(1).gameObject.SetActive(true);
            SelectionCharacters.hasChangedCharacter = false;
            ShowGUICharacter();
        }
    }
}
