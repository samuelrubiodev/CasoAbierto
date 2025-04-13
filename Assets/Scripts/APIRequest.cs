using GroqApiLibrary;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using OpenAI.Chat;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;
using System.Threading.Tasks;
using TMPro;
using FeatureRequest;
using Newtonsoft.Json.Linq;

public class APIRequest : MonoBehaviour
{
    private string openRouterApiKey;
    private string groqApiKey;
    private string togetherApiKey;
    private string elevenlabsApiKey;
    public TMP_Text textoSubtitulos;

    public static string DATOS_CASO = "";

    public static List<ChatMessage> chatMessages = new ();

    void Start()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
        groqApiKey = ApiKey.API_KEY_GROQ;
        elevenlabsApiKey = ApiKey.API_KEY_ELEVENLABS;
        togetherApiKey = ApiKey.API_KEY_TOGETHER;
    }

    private async Task MakeRequestOpenRouter(string prompt, APIRequestElevenLabs aPIRequestElevenLabs)
    {
        RequestOpenRouter requestOpenRouter = new (textoSubtitulos);
        string message = await Task.Run(async () => await requestOpenRouter.SendRequest(prompt));

        bool isMan = MenuPersonajes.personajeSeleccionado.sexo == "Masculino";
        aPIRequestElevenLabs.StreamAudio(message, isMan);

        new MessageStyleManager(textoSubtitulos).SetStyle();
        StartCoroutine(new UIMessageManager(textoSubtitulos).ShowMessage(message));
    }

    private async Task<JObject> SeHaTerminado()
    {
        GameStatus gameStatus = new ();
        return await Task.Run(async () => await gameStatus.SendRequest(prompt: "Analiza esta conversacion: \n"));
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
            string prompt = new ConversationPrompt().CreatePrompt(result?["text"]?.ToString()).ToString();
            await MakeRequestOpenRouter(prompt,aPIRequestElevenLabs);
        } else {
            string prompt = new ConversationPrompt().CreatePrompt(texto).ToString();
            await MakeRequestOpenRouter(prompt,aPIRequestElevenLabs);
        }

        JObject jsonGameStatus = await SeHaTerminado();
        Debug.Log(jsonGameStatus.ToString());

        JObject jsonEmotionalState = await RequestEmotionalState();
        Debug.Log(jsonEmotionalState.ToString());
    }
}
