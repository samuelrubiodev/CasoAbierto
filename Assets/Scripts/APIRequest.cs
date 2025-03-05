using GroqApiLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using TMPro;
using Utilities.Extensions;

public class APIRequest : MonoBehaviour
{
    public string promptWhisper {  get; set; }
    public string promptLLama {  get; set; }

    private string openRouterApiKey;
    private string groqApiKey;
    private string elevenlabsApiKey;
    public TMP_Text textoSubtitulos;

    public static string PROMPT_SYSTEM_CONVERSACION = @"[Contexto del Juego]
        Estás en un juego de investigación policial llamado ""Caso Abierto"". El jugador asume el rol de un detective encargado de resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias. El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

        [Objetivo]
        Tu rol es **exclusivamente** generar respuestas de los personajes dentro del juego. El jugador está interrogando a un personaje y tu trabajo es responder como ese personaje. 
            **No hables como la IA**. Responde **siempre** en el papel del personaje.  
            Si el jugador selecciona a un personaje muerto o desaparecido, cambia automáticamente a un personaje disponible. **No expliques por qué**
            En el mensaje contendrá el personaje seleccionado y el mensaje del jugador con el que interactuar.

        [Instrucciones Adicionales]
        1. **El jugador es el investigador**. No lo llames ""usuario"" ni hagas referencia a que es un juego.  
        2. Responde a lo que el personaje respondería basándose en las evidencias y el contexto del caso. Solo responde con texto.
        3. **Si el personaje se contradice, mantenlo deliberado** para que el jugador deba descubrirlo. No aclares que hay contradicciones.  
        4. Si el jugador hace preguntas irrelevantes o fuera del caso, responde de manera evasiva o con frustración, como lo haría el personaje.  
        5. No contestes con emoticonos, respnde solamente con texto.

        [IMPORTANTE]
        1.Es importante que no envíes mas de 200 caracteres en la respuesta del personaje.

        Estos son los datos del caso, con todos los personajes, evidencias y detalles relevantes:";

    public static string DATOS_CASO = "";
    private static string PROMPT_SYSTEM_ANALISIS = @"
        Analiza la conversacion entre el usuario y el NPC y responde con el booleano 'seHaTerminado' en true o false dependiendo si se ha terminado la conversacion o no.";

    private static string PROMPT_SYSTEM_ANALISIS_EVIDENCIA = @"
        Analiza la evidencia y entrega un resultado técnico y policial detallado
        En este analisis no tienes que explicar como haces el analisis, simplemente da la informacion sin rodeos de forma directa pero ten en cuenta que no debes copiar lo mismo que aparece 'analisis' de la evidencia
        ya que ese analisis es un analisis superficial rapido y tu tienes que hacer un analisis mas profundo, algo que pueda ayudar al jugador a resolver el caso con esta pista pero asegurate de no superar 
        los 300 caracteres. Usa un enfoque forense profundo basado en el contexto del caso:";

    private static List<ChatMessage> chatMessages = new ();

    void Start()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
        groqApiKey = ApiKey.API_KEY_GROQ;
        elevenlabsApiKey = ApiKey.API_KEY_ELEVENLABS;
    }

    private async Task MakeRequestOpenRouter(string prompt, APIRequestElevenLabs aPIRequestElevenLabs)
    {
        if (!chatMessages.Any(x => x is SystemChatMessage))
        {
            chatMessages.Add(new SystemChatMessage(PROMPT_SYSTEM_CONVERSACION + " " + DATOS_CASO));
        }

        chatMessages.Add(new UserChatMessage(prompt));

        try
        {
            OpenAIClientOptions openAIClientOptions = new ()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new (new ApiKeyCredential(openRouterApiKey), openAIClientOptions);

            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = 
                client.GetChatClient("google/gemini-2.0-flash-exp:free").CompleteChatStreamingAsync(chatMessages);

            StringBuilder mensajePersonajeBuilder = new();

            string mensajeCompleto = "";
            
            await foreach (StreamingChatCompletionUpdate update in completionUpdates)
            {
                if (update.ContentUpdate.Count > 0)
                {
                    string texto = update.ContentUpdate[0].Text;
                    mensajePersonajeBuilder.Append(texto);
                    mensajeCompleto += texto;

                    /*
                    string mensajeActual = mensajePersonajeBuilder.ToString();
                    if (texto.Contains('.') || texto.Contains('!') || texto.Contains('?') || mensajePersonajeBuilder.Length > 5 
                        && !aPIRequestElevenLabs.GetAudioSource().isPlaying)
                    {
                        string mensajeActual = mensajePersonajeBuilder.ToString();
                        aPIRequestElevenLabs.StreamAudio(mensajeActual);
                        mensajePersonajeBuilder.Clear();
                    }
                    */
                }
            }
            
            Debug.Log(mensajeCompleto);

            string[] strings = mensajeCompleto.Split(' ');
            textoSubtitulos.SetActive(true);

            textoSubtitulos.outlineColor = Color.black;
            textoSubtitulos.outlineWidth = 0.5f;

            StringBuilder buffer = new();

            aPIRequestElevenLabs.StreamAudio(mensajeCompleto);

            for (int i = 0; i < strings.Length; i++)
            {
                buffer.Append(strings[i] + " ");
                if (i % 5 == 0)
                {
                    textoSubtitulos.text = buffer.ToString();
                    buffer.Clear();
                    await Task.Delay(2000);
                }
                else if (i == strings.Length - 1)
                {
                    textoSubtitulos.text = buffer.ToString();
                    await Task.Delay(2000);
                }
            }

            textoSubtitulos.SetActive(false);
            chatMessages.Add(new AssistantChatMessage(mensajeCompleto));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public bool HayPalabras(string mensaje)
    {
        string[] strings = mensaje.Split(' ');
        return strings.Length > 2;
    }

    private bool SeHaTerminado()
    {
        try
        {
            string jsonSchema = @"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""mensajes"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""seHaTerminado"": { ""type"": ""boolean"", ""description"": ""Indica si el personaje ha terminado de hablar"" }
                        },
                        ""required"": [""seHaTerminado""],
                        ""additionalProperties"": false
                    }
                },
                ""required"": [""personajeActual"",""mensajes""],
                ""additionalProperties"": false
            }";

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
                new SystemChatMessage(PROMPT_SYSTEM_ANALISIS),
                new UserChatMessage("Analiza esta conversacion: \n" + conversacion)
            };

            OpenAIClientOptions openAIClientOptions = new ()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new (new ApiKeyCredential(openRouterApiKey), openAIClientOptions);

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                        jsonSchemaFormatName: "message_data",
                        jsonSchema: BinaryData.FromString(jsonSchema),
                jsonSchemaIsStrict: true)
            };

            ChatCompletion completion = client.GetChatClient("google/gemini-2.0-flash-exp:free").CompleteChat(mensajes, options);

            using JsonDocument jsonDocument = JsonDocument.Parse(completion.Content[0].Text);

            return jsonDocument.RootElement.GetProperty("mensajes").GetProperty("seHaTerminado").GetBoolean();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        return false;
    }

    private JObject CrearPrompt(string prompt, Jugador jugador)
    {
        Caso caso = jugador.casos[Jugador.indexCaso];

        return new JObject
        {
            ["personajeActual"] = new JObject
            {
                ["nombre"] = caso.personajes[0].nombre,
                ["rol"] = caso.personajes[0].rol,
                ["estado"] = caso.personajes[0].estado,
                ["descripcion"] = caso.personajes[0].descripcion,
                ["estado_emocional"] = caso.personajes[0].estadoEmocional
            },
            ["evidenciaSeleccionada"] = new JObject(),
            ["mensajes"] = new JObject
            {
                ["mensajeUsuario"] = prompt,
            }
        };
    }

    public string AnalizarEvidencia(Evidencia evidencia)
    {
        string jsonSchema = @"
        {
            ""type"": ""object"",
            ""properties"": {
                ""evidencia"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""evidenciaSeleccionada"": { ""type"": ""string"", ""description"": ""Nombre de la evidencia seleccionada"" },
                        ""tipoAnalisis"": { ""type"": ""string"", ""description"": ""Tipo de analisis a realizar, huellas daactilares por ejemplo"" },
                        ""resultadoAnalisis"": { ""type"": ""string"", ""description"": ""Analisis de la evidencia"" }
                    },
                    ""required"": [""evidenciaSeleccionada"",""tipoAnalisis"",""resultadoAnalisis""],
                    ""additionalProperties"": false
                }
            },
            ""required"": [""evidencia""],
            ""additionalProperties"": false
        }";


        List<ChatMessage> mensajes = new()
        {
            new SystemChatMessage(PROMPT_SYSTEM_ANALISIS_EVIDENCIA + DATOS_CASO),
            new UserChatMessage("Analiza esta evidencia: " + evidencia.nombre)
        };

        OpenAIClientOptions openAIClientOptions = new()
        {
            Endpoint = new Uri("https://openrouter.ai/api/v1")
        };

        OpenAIClient client = new(new ApiKeyCredential(openRouterApiKey), openAIClientOptions);

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "evidencia_data",
                    jsonSchema: BinaryData.FromString(jsonSchema),
            jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = client.GetChatClient("google/gemini-2.0-flash-exp:free").CompleteChat(mensajes, options);

        return completion.Content[0].Text;
    }

    public async Task incializarAPITexto(APIRequestElevenLabs aPIRequestElevenLabs)
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
        
        string analisis = AnalizarEvidencia(Jugador.jugador.casos[Jugador.indexCaso].evidencias[1]);
        Debug.Log(analisis);
        /*
        string prompt = CrearPrompt(result?["text"]?.ToString(), Jugador.jugador).ToString();
        await MakeRequestOpenRouter(prompt,aPIRequestElevenLabs);

        string mensajePersonaje = json["mensajes"]?["respuestaPersonaje"]?.ToString();
        chatMessages.Add(new AssistantChatMessage(mensajePersonaje));
        aPIRequestElevenLabs.StreamAudio(mensajePersonaje);

        */
    }
}
