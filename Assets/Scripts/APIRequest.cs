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
using System.Collections;
using OpenAI.Images;

public class APIRequest : MonoBehaviour
{
    private string openRouterApiKey;
    private string groqApiKey;
    private string togetherApiKey;
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
        2.Si el jugador no tiene evidencias seleccionadas y te pregunta por una evidencia no tienes que hacer referencia a esa evidencia ya que el personaje no tiene conocimiento de ella a no ser que el personaje este involucrado en esa evidencia.
        3.Si el jugado selecciona una evidencia y te pregunta por ella, debes responder con la información que el personaje tenga sobre esa evidencia.
        4.Solo envía texto, nada de JSON.
       
        Estos son los datos del caso, con todos los personajes, evidencias y detalles relevantes:";

    public static string DATOS_CASO = "";
    private static readonly string PROMPT_SYSTEM_ANALISIS = @"
        Analiza la conversacion entre el usuario y el NPC y responde con el booleano 'seHaTerminado' en true o false.
        Si consideras que el personaje y el jugador han terminado de hablar, que no hay nada más que decir.";

    private static readonly string PROMPT_SYSTEM_ANALISIS_EVIDENCIA = @"
        Analiza la evidencia con un enfoque forense técnico y profundo.
        Lo que tienes que hacer es un analisis mas exhaustivo de la evidencia seleccionada por el jugador ya que anteriormente se hizo un analisis rapido de la evidencia.
        Por favor es muy importante que no copies la propiedad 'analisis' de la evidencia seleccionada ya que el jugador ya tiene esa informacion, debes hacer un analisis mas profundo de la evidencia.
        Tienes que revelar cualquier tipo de información relevante que no se sepa todavía, que pueda ayudar al jugador a resolver el caso, de quien son las huellas, si hay sangre, si hay ADN, quien aparece en las camaras de seguridad, cualquier información valiosa, etc...
        
        Por ultimo es importante que no envíes mas de 200 caracteres.";

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
            chatMessages.Add(new SystemChatMessage(PROMPT_SYSTEM_CONVERSACION + " " + DATOS_CASO));
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
                if (update.ContentUpdate.Count > 0)
                {
                    mensajePersonajeBuilder.Append(update.ContentUpdate[0].Text);
                }
            }
            string mensajeCompleto = mensajePersonajeBuilder.ToString();
            bool isMan = MenuPersonajes.personajeSeleccionado.sexo == "Masculino";

            aPIRequestElevenLabs.StreamAudio(mensajeCompleto,isMan);
            
            StartCoroutine(DisplaySubtitlesCoroutine(mensajeCompleto));
            chatMessages.Add(new AssistantChatMessage(mensajeCompleto));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private IEnumerator DisplaySubtitlesCoroutine(string mensajeCompleto)
    {
        textoSubtitulos.SetActive(true);
        textoSubtitulos.outlineColor = Color.black;
        textoSubtitulos.outlineWidth = 0.5f;

        string[] words = mensajeCompleto.Split(' ');
        int chunkSize = 5;

        for (int i = 0; i < words.Length; i += chunkSize)
        {
            string chunk = string.Join(" ", words.Skip(i).Take(chunkSize));
            textoSubtitulos.text = chunk;
            
            float delay = Mathf.Clamp(chunk.Length * 0.1f, 2f, 5f);
            yield return new WaitForSeconds(delay);
        }

        textoSubtitulos.SetActive(false);
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
