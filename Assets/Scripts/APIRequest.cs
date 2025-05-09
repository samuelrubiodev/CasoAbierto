using GroqApiLibrary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using OpenAI.Chat;
using Task = System.Threading.Tasks.Task;
using System.Threading.Tasks;
using TMPro;
using FeatureRequest;
using Newtonsoft.Json.Linq;
using System;
using Utilities.Extensions;
using UnityEngine.SceneManagement;
using System.Collections;

public class APIRequest : MonoBehaviour
{
    private string openRouterApiKey;
    private string groqApiKey;
    private string togetherApiKey;
    private string elevenlabsApiKey;
    public TMP_Text textoSubtitulos;
    public ErrorOverlayUI errorOverlayUI;   
    public static Dictionary<int, Personaje> characters = new ();

    public static string DATOS_CASO = "";

    void Start()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
        groqApiKey = ApiKey.API_KEY_GROQ;
        elevenlabsApiKey = ApiKey.API_KEY_ELEVENLABS;
        togetherApiKey = ApiKey.API_KEY_TOGETHER;
        OpenRouterImpl.ResetInstance(textoSubtitulos);
        ElevenLabsImpl.ResetInstance(textoSubtitulos);

        for (int i = 0; i < Caso.caso.personajes.Count; i++)
        {
            characters.Add(Caso.caso.personajes[i].id, Caso.caso.personajes[i]);
        }
    }

    private async Task MakeRequestOpenRouter(string prompt, APIRequestElevenLabs aPIRequestElevenLabs)
    {
        RequestOpenRouter requestOpenRouter = RequestOpenRouter.GetInstance();
        string message = await Task.Run(async () => await requestOpenRouter.SendRequest(prompt));

        bool isMan = SelectionCharacters.selectedCharacter.sexo.ToLower() == "masculino" ? true : false;
        aPIRequestElevenLabs.StreamAudio(message, isMan);

        new MessageStyleManager(textoSubtitulos).SetStyle();
        StartCoroutine(new UIMessageManager(textoSubtitulos).ShowMessage(message));
    }

    private async Task<JObject> RequestEmotionalState()
    {
        CharacterEmotionalState characterEmotionalState = new ();
        return await Task.Run(async () => await characterEmotionalState.SendRequest(prompt: "Analiza esta conversacion: \n"));
    }

    public async Task<string> AnalizarEvidencia(Evidencia evidencia)
    {
        AnalysisEvidence analysisEvidence = new ();
        string message = await Task.Run(async () => await analysisEvidence.SendRequest(prompt: "Analiza esta evidencia: " + evidencia.nombre));
        return message;
    }

    public async Task RequestAPI(APIRequestElevenLabs aPIRequestElevenLabs, string texto)
    {
        string prompt = "";

        CallBackButton tryButton = OnTryButtonClicked;
        errorOverlayUI.SetTryCallBackButtonDelegate(tryButton);

        CallBackButton mainButton = OnMainMenu;
        errorOverlayUI.SetMainMenuCallBackButtonnDelegate(mainButton);

        UtilitiesErrorMessage errorMessage = new ("ErrorMessages/ErrorMessage");
        ErrorsMessage errors = errorMessage.ReadJSON();

        try {
            if (texto == "")
            {
                var groqApi = new GroqApiClient(groqApiKey, "https://api.groq.com/openai/v1");
                var audioStream = File.OpenRead(Application.persistentDataPath + "/audio.wav");
                var result = await groqApi.CreateTranscriptionAsync (
                    audioStream,
                    "audio.wav",
                    "whisper-large-v3-turbo",
                    prompt: "Transcribe este audio de esta persona",
                    language: "es"
                );
                prompt = new ConversationPrompt().CreatePrompt(result?["text"]?.ToString()).ToString();
            } else {
                prompt = new ConversationPrompt().CreatePrompt(texto).ToString();
            }

            try {
                await MakeRequestOpenRouter(prompt,aPIRequestElevenLabs);
                /*
                JObject jsonEmotionalState = await RequestEmotionalState();
                Debug.Log(jsonEmotionalState.ToString()); 
                */
            } catch (Exception ex) {
                Debug.Log(ex.Message);
                this.SetActive(false);

                System.Random random = new ();
                int randomTitle = random.Next(0, errors.ia.titulos.Count);
                int randomMessage = random.Next(0, errors.ia.mensajes.Count);

                errorOverlayUI.ShowError(errors.ia.titulos[randomTitle], errors.ia.mensajes[randomMessage]);
            }
        } catch (Exception ex) {
            Debug.Log(ex.Message);
            this.SetActive(false);

            System.Random random = new ();
            int randomTitle = random.Next(0, errors.audio.titulos.Count);
            int randomMessage = random.Next(0, errors.audio.mensajes.Count);

            errorOverlayUI.ShowError(errors.audio.titulos[randomTitle], errors.audio.mensajes[randomMessage]);
        }
    }

    public void OnMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OnTryButtonClicked()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        this.SetActive(true);
        errorOverlayUI.HideMenu();
    }
}
