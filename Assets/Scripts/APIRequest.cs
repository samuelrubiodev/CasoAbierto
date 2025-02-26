using GroqApiLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using UnityEditor.VersionControl;
using Task = System.Threading.Tasks.Task;
using System.Linq;

public class APIRequest : MonoBehaviour
{
    public string promptWhisper {  get; set; }
    public string promptLLama {  get; set; }

    private string openRouterApiKey;
    private string groqApiKey;
    private string elevenlabsApiKey;

    private static List<ChatMessage> chatMessages = new List<ChatMessage>();
    
    private async Task<JObject> CrearPromptSystem()
    {
        SQLiteManager sqliteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqliteManager.crearConexion();
        RedisManager redisManager = await sqliteManager.GetRedisManager();
        redisManager.crearConexion();

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
            },
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

    private string MakeRequestOpenRouter(string prompt)
    {
        if (!chatMessages.Any(x => x is SystemChatMessage))
        {
            string promptSistema = @"[Contexto del Juego]
                Estás en un juego de investigación policial llamado ""Caso Abierto"". El jugador asume el rol de un detective encargado de resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias. El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

                [Objetivo]
                Tu rol es **exclusivamente** generar respuestas de los personajes dentro del juego. El jugador está interrogando a un personaje y tu trabajo es responder como ese personaje en el campo ""respuestaPersonaje"" de ""mensajes"".   
                    **No hables como la IA**. Responde **siempre** en el papel del personaje.  
                    Si el jugador selecciona a un personaje muerto o desaparecido, cambia automáticamente a un personaje disponible. **No expliques por qué**

                [Instrucciones Adicionales]
                1. **El jugador es el investigador**. No lo llames ""usuario"" ni hagas referencia a que es un juego.  
                2. `respuestaPersonaje` debe reflejar lo que el personaje respondería basándose en las evidencias y el contexto del caso.
                3.    Cuando el jugador hable tendrás que poner de forma automatica en el valor de 'mensajeUsuario'
                4. **Si el personaje se contradice, mantenlo deliberado** para que el jugador deba descubrirlo. No aclares que hay contradicciones.  
                5. **SeHaTerminado**: Cambia a true solo cuando el personaje no tenga más información relevante o el jugador haya presionado suficiente. No expliques cuándo cambia; simplemente hazlo.  
                6. Si el jugador selecciona un personaje muerto/desaparecido, **selecciona automáticamente** uno vivo. No justifiques el cambio.  
                7. Si el jugador hace preguntas irrelevantes o fuera del caso, responde de manera evasiva o con frustración, como lo haría el personaje.  
                8. No cambies el mensaje que hace el usuario de ""mensajeUsuario""
                9. Validación**: Asegúrate de que el JSON pueda ser parseado sin errores. 

                Estos son los datos del caso, con todos los personajes, evidencias y detalles relevantes:" + CrearPromptSystem().ToString();

            chatMessages.Add(new SystemChatMessage(promptSistema));
        }

        string jsonSchema = @"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""datosJugador"": {
                        ""type"": ""object"",
                        ""properties"": {
                             ""nombre"": { ""type"": ""string"", ""description"": ""Nombre del jugador"" },
                             ""estado"": { ""type"": ""string"", ""description"": ""Estado del jugador, Activo o Inactivo"" },
                             ""progreso"": { ""type"": ""string"", ""description"": ""En que caso va, nombre del caso"" }
                         },
                         ""required"": [""nombre"",""estado"",""progreso""],
                         ""additionalProperties"": false
                    },
                    ""Caso"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""tituloCaso"": { ""type"": ""string"", ""description"": ""Titulo del caso"" },
                            ""descripcionCaso"": { ""type"": ""string"", ""description"": ""Descripción del caso"" },
                            ""fechaOcurrido"": { ""type"": ""string"", ""description"": ""YYYY-MM-DD"" },
                            ""lugar"": { ""type"": ""string"", ""description"": ""Lugar en el que ha ocurrido el caso"" },
                            ""tiempoRestante"": {""type"": ""string"", ""description"": ""HH:MM"" },
                            ""personajeActual"": {""type"": ""string"", ""description"": ""Personaje actual del usuario""},
                            ""mensajes"": {
                                ""type"": ""object"",
                                ""properties"": {
                                    ""mensajeUsuario"": {""type"": ""string"", ""description"": ""Mensaje del usuario""},
                                    ""respuestaPersonaje"": {""type"": ""string"", ""description"": ""Mensaje del personaje""},
                                },
                                ""required"": [""mensajeUsuario"",""respuestaPersonaje""],
                                ""additionalProperties"": false
                            }
                        },
                        ""required"": [""tituloCaso"",""descripcionCaso"",""fechaOcurrido"",""lugar"",""tiempoRestante"",""personajeActual""],
                        ""additionalProperties"": false
                    }
                },
                ""required"": [""datosJugador"",""Caso""],
                ""additionalProperties"": false
            }";

        chatMessages.Add(new UserChatMessage(prompt));

        try
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new OpenAIClient(new ApiKeyCredential(openRouterApiKey), openAIClientOptions);

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                        jsonSchemaFormatName: "message_data",
                        jsonSchema: BinaryData.FromString(jsonSchema),
                jsonSchemaIsStrict: true),
            };

            ChatCompletion completion = client.GetChatClient("google/gemini-2.0-flash-001").CompleteChat(chatMessages, options);

            using JsonDocument jsonDocument = JsonDocument.Parse(completion.Content[0].Text);

            return jsonDocument.RootElement.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        return null;
    }

    private JObject CrearPrompt(string prompt, Jugador jugador)
    {
        Caso caso = jugador.casos[Jugador.indexCaso];

        return new JObject
        {
            ["datosJugador"] = new JObject
            {
                ["nombre"] = jugador.nombre,
                ["estado"] = jugador.estado,
                ["progreso"] = jugador.progreso
            },
            ["Caso"] = new JObject
            {
                ["tituloCaso"] = caso.tituloCaso,
                ["descripcionCaso"] = caso.descripcion,
                ["dificultad"] = caso.dificultad,
                ["fechaOcurrido"] = caso.fechaOcurrido,
                ["lugar"] = caso.lugar,
                ["tiempoRestante"] = caso.tiempoRestante,

                ["personajeActual"] = new JArray
                {
                    new JObject
                    {
                        ["nombre"] = caso.personajes[0].nombre,
                        ["rol"] = caso.personajes[0].rol,
                        ["estado"] = caso.personajes[0].estado,
                        ["descripcion"] = caso.personajes[0].descripcion,
                        ["estado_emocional"] = caso.personajes[0].estadoEmocional
                    },
                },
                ["mensajes"] = new JObject
                {
                    ["mensajeUsuario"] = prompt,
                    ["respuestaPersonaje"] = "Respuesta del personaje seleccionado, cuando el jugador tenga seleccionado un personaje",
                    ["seHaTerminado"] = false
                }
            },
        };
    }

    public async Task incializarAPITexto()
    {
        SQLiteManager sqliteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqliteManager.crearConexion();

        openRouterApiKey = sqliteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey;
        elevenlabsApiKey = sqliteManager.GetAPIS()[ApiKey.ELEVENLABS].apiKey;
        groqApiKey = sqliteManager.GetAPIS()[ApiKey.GROQ].apiKey;

        var groqApi = new GroqApiClient(groqApiKey, "https://api.groq.com/openai/v1");
        var audioStream = File.OpenRead(Application.persistentDataPath + "/audio.wav");
        var result = await groqApi.CreateTranscriptionAsync (
            audioStream,
            "audio.wav",
            "whisper-large-v3-turbo",
            prompt: "Transcribe este audio de esta persona",
            language: "es"
        );

        string jsonResponseLlama = MakeRequestOpenRouter(result?["text"]?.ToString());
        
        JObject json = JObject.Parse(jsonResponseLlama);

        JArray jarray = (JArray)json["choices"];
        JObject firstChoice = (JObject)jarray[0];
        JObject message = (JObject)firstChoice["message"];

        JObject mensajePersonaje = JObject.Parse(message["content"].ToString());

        chatMessages.Add(new AssistantChatMessage(mensajePersonaje.ToString()));

        promptLLama = mensajePersonaje["Caso"]?["mensajes"]?["respuestaPersonaje"]?.ToString();
    }

}
