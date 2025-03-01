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

public class APIRequest : MonoBehaviour
{
    public string promptWhisper {  get; set; }
    public string promptLLama {  get; set; }

    private string openRouterApiKey;
    private string groqApiKey;
    private string elevenlabsApiKey;

    private static string PROMPT_SYSTEM_CONVERSACION = @"[Contexto del Juego]
        Estás en un juego de investigación policial llamado ""Caso Abierto"". El jugador asume el rol de un detective encargado de resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias. El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

        [Objetivo]
        Tu rol es **exclusivamente** generar respuestas de los personajes dentro del juego. El jugador está interrogando a un personaje y tu trabajo es responder como ese personaje. 
            **No hables como la IA**. Responde **siempre** en el papel del personaje.  
            Si el jugador selecciona a un personaje muerto o desaparecido, cambia automáticamente a un personaje disponible. **No expliques por qué**

        [Instrucciones Adicionales]
        1. **El jugador es el investigador**. No lo llames ""usuario"" ni hagas referencia a que es un juego.  
        2. Responde a lo que el personaje respondería basándose en las evidencias y el contexto del caso. Solo responde con texto.
        3. **Si el personaje se contradice, mantenlo deliberado** para que el jugador deba descubrirlo. No aclares que hay contradicciones.  
        4. Si el jugador hace preguntas irrelevantes o fuera del caso, responde de manera evasiva o con frustración, como lo haría el personaje.  
        5. No contestes con emoticonos, respnde solamente con texto.

        [IMPORTANTE]
        1.Es importante que no envíes mas de 200 caracteres en la respuesta del personaje.

        Estos son los datos del caso, con todos los personajes, evidencias y detalles relevantes:";

    private static string PROMPT_SYSTEM_ANALISIS = @"
        Analiza la conversacion entre el usuario y el NPC y responde con el booleano 'seHaTerminado' en true o false dependiendo si se ha terminado la conversacion o no.";

    private static List<ChatMessage> chatMessages = new ();

    void Start()
    {
        openRouterApiKey = ApiKey.API_KEY_OPEN_ROUTER;
        groqApiKey = ApiKey.API_KEY_GROQ;
        elevenlabsApiKey = ApiKey.API_KEY_ELEVENLABS;
    }

    private JObject CrearPromptSystem()
    {

        Jugador jugador1 = Jugador.jugador;
        Caso caso = jugador1.casos[Jugador.indexCaso];

        JObject objetoJson = new()
        {
            ["datosJugador"] = new JObject
            {
                ["_comentario"] = "Datos importantes del jugador, no cambies el nombre del jugador",
                ["_estado"] = "Activo o Inactivo",
                ["_progreso"] = "En que caso va, poner nombre del caso",
                ["nombre"] = jugador1.nombre,
                ["estado"] = jugador1.estado,
                ["progreso"] = jugador1.progreso
            },
            ["Caso"] = new JObject
            {
                ["_comentario"] = "Datos del caso actual",
                ["tituloCaso"] = caso.tituloCaso,
                ["descripcionCaso"] = caso.descripcion,
                ["dificultad"] = caso.dificultad,
                ["fechaOcurrido"] = caso.fechaOcurrido,
                ["lugar"] = caso.lugar,
                ["tiempoRestante"] = caso.tiempoRestante,

                ["cronologia"] = ObtenerCronologias(caso.cronologia),
                ["evidencias"] = ObtenerEvidencias(caso.evidencias),
                ["personajes"] = ObtenerPersonajes(caso.personajes),
                ["explicacionCasoResuelto"] = caso.explicacionCasoResuelto
            }
        };

        return objetoJson;
    }

    private JArray ObtenerPersonajes(List<Personaje> personajesLista)
    {
        var personajes = new List<Dictionary<string, string>>();

        foreach (Personaje personaje in personajesLista)
        {
            var personajeDiccionario = new Dictionary<string, string>
            {
                { "nombre", personaje.nombre },
                { "rol", personaje.rol },
                { "estado", personaje.estado },
                { "descripcion", personaje.descripcion },
                { "estado_emocional", personaje.estadoEmocional }
            };

            personajes.Add(personajeDiccionario);
        }
       
        var objeto = JsonConvert.SerializeObject(personajes);
        return JArray.Parse(objeto);
    }

    private JArray ObtenerEvidencias(List<Evidencia> evidenciasLista)
    {
        var evidencias = new List<Dictionary<string, string>>();

        foreach (Evidencia evidencia in evidenciasLista)
        {
            var evidenciaDiccionario = new Dictionary<string, string>
            {
                { "nombre", evidencia.nombre },
                { "descripcion", evidencia.descripcion },
                { "tipo", evidencia.tipo },
                { "analisis", evidencia.analisis },
                {"ubicacion", evidencia.ubicacion }
            };

            evidencias.Add(evidenciaDiccionario);
        }
        
        var objeto = JsonConvert.SerializeObject(evidencias);
        return JArray.Parse(objeto);
    }

    private JArray ObtenerCronologias(List<Cronologia> cronologiasLista)
    {
        var cronologias = new List<Dictionary<string, string>>();

        foreach (Cronologia cronologia in cronologiasLista)
        {
            var cronologiaDiccionario = new Dictionary<string, string>
            {
                { "fecha", cronologia.fecha.ToString() },
                { "hora", cronologia.hora },
                { "evento", cronologia.evento }
            };

            cronologias.Add(cronologiaDiccionario);
        }

        var objeto = JsonConvert.SerializeObject(cronologias);
        return JArray.Parse(objeto);
    }

    private async Task<string> MakeRequestOpenRouter(string prompt, APIRequestElevenLabs aPIRequestElevenLabs)
    {
        if (!chatMessages.Any(x => x is SystemChatMessage))
        {
            string promptSistema = PROMPT_SYSTEM_CONVERSACION + CrearPromptSystem().ToString();
            chatMessages.Add(new SystemChatMessage(promptSistema));
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

                    if (texto.Contains('.') || texto.Contains('!') || texto.Contains('?') || mensajePersonajeBuilder.Length > 5)
                    {
                        string mensajeActual = mensajePersonajeBuilder.ToString();
                        aPIRequestElevenLabs.StreamAudio(mensajeActual);
                        mensajePersonajeBuilder.Clear();
                    }
                }
            }

            Debug.Log(mensajeCompleto);

            chatMessages.Add(new AssistantChatMessage(mensajeCompleto));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        return null;
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

    // Este metodo hace crashear el juego, se debe de revisar
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
                ["respuestaPersonaje"] = "Respuesta del personaje seleccionado, cuando el jugador tenga seleccionado un personaje",
                ["seHaTerminado"] = false
            }
        };
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

        //string prompt = CrearPrompt(result?["text"]?.ToString(), Jugador.jugador).ToString();

        await MakeRequestOpenRouter(result?["text"]?.ToString(),aPIRequestElevenLabs);

        /*
        string mensajePersonaje = json["mensajes"]?["respuestaPersonaje"]?.ToString();
        chatMessages.Add(new AssistantChatMessage(mensajePersonaje));
        aPIRequestElevenLabs.StreamAudio(mensajePersonaje);

        */
    }
}
