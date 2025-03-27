using GroqApiLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using TMPro;

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
        if (!chatMessages.Any(x => x is SystemChatMessage))
        {
            chatMessages.Add(new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_CONVERSATION + " " + DATOS_CASO));
        }

        chatMessages.Add(new UserChatMessage(prompt));

        try
        {  
            ChatManager chatManager = new (openRouterApiKey, chatMessages);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions();
            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatManager.CreateStremingChat(ChatManager.CHAT_MODEL, options);

            StringBuilder mensajePersonajeBuilder = new();
            await foreach (StreamingChatCompletionUpdate update in completionUpdates)
            {
                if (update.ContentUpdate.Count > 0) mensajePersonajeBuilder.Append(update.ContentUpdate[0].Text);
            }
            string mensajeCompleto = mensajePersonajeBuilder.ToString();
            bool isMan = MenuPersonajes.personajeSeleccionado.sexo == "Masculino";

            aPIRequestElevenLabs.StreamAudio(mensajeCompleto,isMan);
            
            StartCoroutine(new UIMessageManager(textoSubtitulos).ShowMessage(mensajeCompleto));
            chatMessages.Add(new AssistantChatMessage(mensajeCompleto));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private bool SeHaTerminado()
    {
        try
        {
            string jsonSchema = Schemas.CONVERSATIONS;

            string conversacion = "";

            foreach (var message in chatMessages)
            {
                if (message is UserChatMessage userMessage)
                {
                    conversacion += "Usuario: " + userMessage.Content[0].Text + "\n";
                }
                else if (message is AssistantChatMessage aiMessage)
                {
                    conversacion += "NPC: " + aiMessage.Content[0].Text + "\n";
                }
            }
            
            List<ChatMessage> mensajes = new() 
            {
                new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_ANALYSIS),
                new UserChatMessage("Analiza esta conversacion: \n" + conversacion)
            };

            ChatManager chatManager = new (openRouterApiKey, chatMessages);
            ChatCompletionOptions options = chatManager.CreateChatCompletionOptions(jsonSchema);
            ChatCompletion completion = chatManager.CreateChat(ChatManager.CHAT_MODEL, options);

            using JsonDocument jsonDocument = JsonDocument.Parse(completion.Content[0].Text);
            return jsonDocument.RootElement.GetProperty("mensajes").GetProperty("seHaTerminado").GetBoolean();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return false;
    }

    private JObject CrearPrompt(string prompt)
    {
        JObject evidenciaSeleccionada = new ();

        Evidencia evidencia = MenuEvidencias.evidenciaSeleccionada;

        if (MenuEvidencias.evidenciaSeleccionada != null)
        {
            evidenciaSeleccionada = new JObject
            {
                ["nombre"] = evidencia.nombre,
                ["descripcion"] = evidencia.descripcion
            };
        }

        JObject personajeSeleccionado = new();

        Personaje personaje = MenuPersonajes.personajeSeleccionado;

        if (MenuPersonajes.personajeSeleccionado != null)
        {
            personajeSeleccionado = new JObject
            {
                ["nombre"] = personaje.nombre,
                ["rol"] = personaje.rol,
                ["estado"] = personaje.estado,
                ["descripcion"] = personaje.descripcion,
                ["estado_emocional"] = personaje.estadoEmocional
            };
        }

        return new JObject
        {
            ["personajeActual"] = personajeSeleccionado,
            ["evidenciaSeleccionada"] = evidenciaSeleccionada,
            ["mensajes"] = new JObject
            {
                ["mensajeUsuario"] = prompt,
            }
        };
    }

    public async Task<string> AnalizarEvidencia(Evidencia evidencia)
    {
        string jsonSchema = Schemas.EVIDENCES;

        List<ChatMessage> mensajes = new()
        {
            new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_ANALYSIS_EVIDENCE + DATOS_CASO),
            new UserChatMessage("Analiza esta evidencia: " + evidencia.nombre)
        };

        ChatManager chatManager = new (openRouterApiKey, mensajes);
        ChatCompletionOptions options = chatManager.CreateChatCompletionOptions(jsonSchema);
        AsyncCollectionResult<StreamingChatCompletionUpdate> completionResult = chatManager.CreateStremingChat(ChatManager.CHAT_MODEL, options);

        string mensajeCompleto = "";
        await foreach (StreamingChatCompletionUpdate update in completionResult)
        {
            if (update.ContentUpdate.Count > 0)
            {
                string texto = update.ContentUpdate[0].Text;
                mensajeCompleto += texto;
            }
        }

        chatMessages.Add(new AssistantChatMessage("El investigador ha analizado esta evidencia y se ha concluido lo siguiente:" + mensajeCompleto));

        return mensajeCompleto;
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
            string prompt = CrearPrompt(result?["text"]?.ToString()).ToString();
            await MakeRequestOpenRouter(prompt,aPIRequestElevenLabs);
        } else {
            string prompt = CrearPrompt(texto).ToString();
            await MakeRequestOpenRouter(prompt,aPIRequestElevenLabs);
        }
    }
}
