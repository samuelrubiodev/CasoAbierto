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

public class APIRequest : MonoBehaviour
{
    public string promptWhisper {  get; set; }
    public string promptLLama {  get; set; }
    public string groqApiKey;
    public string elevenlabsApiKey;

    private static List<object> conversationHistory = new List<object>();
    private static List<ChatMessage> chatMessages = new List<ChatMessage>();
    
    private JObject crearPromptSystem()
    {
        SQLiteManager sqliteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqliteManager.crearConexion();
        RedisManager redisManager = new RedisManager("[IP_REMOVED]", "6379", "[PASSWORD_REMOVED]");
        redisManager.crearConexion();

        long jugadorID = sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer;

        string nombreJugador = "";
        string estadoJugador = "";
        string progresoJugador = "";

        HashEntry[] jugador = redisManager.GetHash($"jugadores:{jugadorID}");

        foreach (HashEntry hashEntry in jugador)
        {
            if (hashEntry.Name == "nombre")
            {
                nombreJugador = hashEntry.Value;
            }
            else if (hashEntry.Name == "estado")
            {
                estadoJugador = hashEntry.Value;
            }
            else if (hashEntry.Name == "progreso")
            {
                progresoJugador = hashEntry.Value;
            }
        }

        string tituloCaso = "";
        string descripcionCaso = "";
        string dificultadCaso = "";
        string fechaOcurrido = "";
        string lugarCaso = "";
        string tiempoRestante = "";
        string explicacionCasoResuelto = "";

        HashEntry[] caso = redisManager.GetHash($"jugadores:{jugadorID}:caso:1");

        foreach (HashEntry hashEntry in caso)
        {
            if (hashEntry.Name == "tituloCaso")
            {
                tituloCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "descripcionCaso")
            {
                descripcionCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "dificultad")
            {
                dificultadCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "fechaOcurrido")
            {
                fechaOcurrido = hashEntry.Value;
            }
            else if (hashEntry.Name == "lugar")
            {
                lugarCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "tiempoRestante")
            {
                tiempoRestante = hashEntry.Value;
            }
            else if (hashEntry.Name == "explicacionCasoResuelto")
            {
                explicacionCasoResuelto = hashEntry.Value;
            }
        }

        JObject objetoJson = new()
        {
            ["datosJugador"] = new JObject
            {
                ["_comentario"] = "Datos importantes del jugador, no cambies el nombre del jugador",
                ["_estado"] = "Activo o Inactivo",
                ["_progreso"] = "En que caso va, poner nombre del caso",
                ["nombre"] = nombreJugador,
                ["estado"] = estadoJugador,
                ["progreso"] = progresoJugador
            },
            ["Caso"] = new JObject
            {
                ["_comentario"] = "Datos del caso actual",
                ["tituloCaso"] = tituloCaso,
                ["descripcionCaso"] = descripcionCaso,
                ["dificultad"] = dificultadCaso,
                ["fechaOcurrido"] = fechaOcurrido,
                ["lugar"] = lugarCaso,
                ["tiempoRestante"] = tiempoRestante,

                ["cronologia"] = obtenerCronologia(redisManager, jugadorID),
                ["evidencias"] = obtenerEvidencias(redisManager, jugadorID),
                ["personajes"] = obtenerPersonajes(redisManager,jugadorID),
                ["explicacionCasoResuelto"] = explicacionCasoResuelto
            },
        };

        return objetoJson;
    }

    private JArray obtenerPersonajes(RedisManager redisManager, long jugadorID)
    {
        var personajes = new List<Dictionary<string, string>>();

        foreach (var key in redisManager.GetServer().Keys(pattern: $"jugadores:{jugadorID}:personajes:*"))
        {
            var type = redisManager.GetDB().KeyType(key);

            if (type == RedisType.Hash)
            {
                var hashPersonajes = redisManager.GetDB().HashGetAll(key);

                var personaje = new Dictionary<string, string>();

                foreach (var entry in hashPersonajes)
                {
                    personaje[entry.Name] = entry.Value;
                }

                personajes.Add(personaje);
            }
        }
        var objeto = JsonConvert.SerializeObject(personajes);
        return JArray.Parse(objeto);
    }

    private JArray obtenerEvidencias(RedisManager redisManager,long jugadorID)
    {
        var evidencias = new List<Dictionary<string, string>>();

        foreach (var key in redisManager.GetServer().Keys(pattern: $"jugadores:{jugadorID}:evidencias:*"))
        {
            var type = redisManager.GetDB().KeyType(key);

            if (type == RedisType.Hash)
            {
                var hashEvidencias = redisManager.GetDB().HashGetAll(key);

                var evidencia = new Dictionary<string, string>();

                foreach (var entry in hashEvidencias)
                {
                    evidencia[entry.Name] = entry.Value;
                }

                evidencias.Add(evidencia);
            }
        }
        var objeto = JsonConvert.SerializeObject(evidencias);
        return JArray.Parse(objeto);
    }

    private JArray obtenerCronologia(RedisManager redisManager,long jugadorID)
    {
        var evidencias = new List<Dictionary<string, string>>();

        foreach (var key in redisManager.GetServer().Keys(pattern: $"jugadores:{jugadorID}:caso:1:cronologia:*"))
        {
            var type = redisManager.GetDB().KeyType(key);

            if (type == RedisType.Hash)
            {
                var hashCronologia = redisManager.GetDB().HashGetAll(key);

                var cronologia = new Dictionary<string, string>();

                foreach (var entry in hashCronologia)
                {
                    cronologia[entry.Name] = entry.Value;
                }
                evidencias.Add(cronologia);
            }
        }
        var objeto = JsonConvert.SerializeObject(evidencias);
        return JArray.Parse(objeto);
    }

    private string MakeRequestAPILlama(string prompt)
    {
        if (!chatMessages.Exists(x => x.ToString().Contains("system")))
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

                Estos son los datos del caso, con todos los personajes, evidencias y detalles relevantes:" + crearPromptSystem().ToString();

            //conversationHistory.Add(new { role = "system", content = promptSistema});
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


        //string promptLlamada = crearPrompt(prompt).ToString();

        //Debug.Log(promptLlamada);

        //conversationHistory.Add(new { role = "user", content = promptLlamada });
        chatMessages.Add(new UserChatMessage(prompt));

        //Debug.Log(JsonConvert.SerializeObject(conversationHistory, Formatting.Indented));

        try
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new OpenAIClient(new ApiKeyCredential("[API_REMOVED]"), openAIClientOptions);

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

    private JObject crearPrompt(string prompt)
    {
        SQLiteManager sqliteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqliteManager.crearConexion();
        RedisManager redisManager = new RedisManager("[IP_REMOVED]", "6379", "[PASSWORD_REMOVED]");
        redisManager.crearConexion();

        string nombreJugador = "";
        string estadoJugador = "";
        string progresoJugador = "";

        HashEntry[] jugador = redisManager.GetHash($"jugadores:{sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer}");

        foreach (HashEntry hashEntry in jugador)
        {
            if (hashEntry.Name == "nombre")
            {
                nombreJugador = hashEntry.Value;
            }
            else if (hashEntry.Name == "estado")
            {
                estadoJugador = hashEntry.Value;
            }
            else if (hashEntry.Name == "progreso")
            {
                progresoJugador = hashEntry.Value;
            }
        }

        string tituloCaso = "";
        string descripcionCaso = "";
        string dificultadCaso = "";
        string fechaOcurrido = "";
        string lugarCaso = "";
        string tiempoRestante = "";

        HashEntry[] caso = redisManager.GetHash($"jugadores:{sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer}:caso:1");

        foreach (HashEntry hashEntry in caso)
        {
            if (hashEntry.Name == "tituloCaso")
            {
                tituloCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "descripcionCaso")
            {
                descripcionCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "dificultad")
            {
                dificultadCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "fechaOcurrido")
            {
                fechaOcurrido = hashEntry.Value;
            }
            else if (hashEntry.Name == "lugar")
            {
                lugarCaso = hashEntry.Value;
            }
            else if (hashEntry.Name == "tiempoRestante")
            {
                tiempoRestante = hashEntry.Value;
            }
        }

        string nombre = string.Empty;
        string estado = string.Empty;
        string estadoEmocional = string.Empty;
        string descripcion = string.Empty;
        string rol = string.Empty;

        HashEntry[] personajes = redisManager.GetHash($"jugadores:{sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer}:personajes:1");

        foreach (HashEntry personaje in personajes)
        {
            if (personaje.Name == "nombre")
            {
                nombre = personaje.Value;
            }
            else if (personaje.Name == "estado")
            {
                estado = personaje.Value;
            }
            else if (personaje.Name == "estado_emocional")
            {
                estadoEmocional = personaje.Value;
            }
            else if (personaje.Name == "descripcion")
            {
                descripcion = personaje.Value;
            }
            else if (personaje.Name == "rol")
            {
                rol = personaje.Value;
            }
        }

        return new JObject
        {
            ["datosJugador"] = new JObject
            {
                ["nombre"] = nombreJugador,
                ["estado"] = estadoJugador,
                ["progreso"] = progresoJugador
            },
            ["Caso"] = new JObject
            {
                ["tituloCaso"] = tituloCaso,
                ["descripcionCaso"] = descripcionCaso,
                ["dificultad"] = dificultadCaso,
                ["fechaOcurrido"] = fechaOcurrido,
                ["lugar"] = lugarCaso,
                ["tiempoRestante"] = tiempoRestante,

                ["personajeActual"] = new JArray
                {
                    new JObject
                    {
                        ["nombre"] = nombre,
                        ["rol"] = rol,
                        ["estado"] = estado,
                        ["descripcion"] = descripcion,
                        ["estado_emocional"] = estadoEmocional
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
        var groqApi = new GroqApiClient("[API_REMOVED]", "https://api.groq.com/openai/v1");
        var audioStream = File.OpenRead(Application.persistentDataPath + "/audio.wav");
        var result = await groqApi.CreateTranscriptionAsync (
            audioStream,
            "audio.wav",
            "whisper-large-v3-turbo",
            prompt: "Transcribe este audio de esta persona",
            language: "es"
        );

        string jsonResponseLlama = MakeRequestAPILlama(result?["text"]?.ToString());
        
        JObject json = JObject.Parse(jsonResponseLlama);

        Debug.Log(json);

        JArray jarray = (JArray)json["choices"];
        JObject firstChoice = (JObject)jarray[0];
        JObject message = (JObject)firstChoice["message"];

        JObject mensajePersonaje = JObject.Parse(message["content"].ToString());

        //conversationHistory.Add(new { role = "assistant", content = mensajePersonaje.ToString()});
        chatMessages.Add(new AssistantChatMessage(mensajePersonaje.ToString()));

        this.promptLLama = mensajePersonaje["Caso"]?["mensajes"]?["respuestaPersonaje"]?.ToString();
    }

}
