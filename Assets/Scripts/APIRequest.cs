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

public class APIRequest : MonoBehaviour
{
    public string promptWhisper {  get; set; }
    public string promptLLama {  get; set; }
    public string groqApiKey;
    public string elevenlabsApiKey;

    private static List<object> conversationHistory = new List<object>();
    
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

    private async Task<string> MakeRequestAPILlama(string prompt)
    {
        if (!conversationHistory.Exists(x => x.ToString().Contains("system")))
        {
            string promptSistema = @"[Contexto del Juego]

                Estás en un juego de investigación policial llamado ""Caso Abierto"". El jugador asume el rol de un detective encargado de resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias. El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

                [Objetivo]
                Tu rol es **exclusivamente** generar respuestas de los personajes dentro del juego. El jugador está interrogando a un personaje y tu trabajo es responder como ese personaje en el campo ""respuestaPersonaje"" de ""mensajes"".   
                    **No expliques el JSON**. Solo genera el contenido solicitado.  
                    **No hables como la IA**. Responde **siempre** en el papel del personaje.  
                    Si el jugador selecciona a un personaje muerto o desaparecido, cambia automáticamente a un personaje disponible. **No expliques por qué

                 **IMPORTANTE**:  
                - `mensajeUsuario` siempre refleja exactamente lo que el jugador dice.  
                - **No modifiques `mensajeUsuario`**. Solo cambia `respuestaPersonaje` en función de lo que el personaje diría.
                - El JSON debe estar **100% bien formado**. **No generes JSON con errores de sintaxis.**  


                [Estructura del JSON de respuesta]
                El JSON que generas sigue este formato:  

                {
                    ""datosJugador"": {
                        ""nombre"": ""Detective Ramírez"",
                        ""estado"": ""Activo"",
                        ""progreso"": ""Caso del Despacho Sellado""
                    },
                    ""Caso"": {
                        ""tituloCaso"": ""El Despacho Sellado"",
                        ""descripcionCaso"": ""Un reconocido abogado fue encontrado muerto en su despacho cerrado con llave desde el interior."",
                        ""fechaOcurrido"": ""2024-11-05"",
                        ""lugar"": ""Bufete 'Martínez & Asociados'"",
                        ""tiempoRestante"": ""35:00"",

                        ""personajeActual"": ""Laura Fernández"",
                        ""mensajes"": {
                            ""mensajeUsuario"": ""¿Qué viste anoche?"",
                            ""respuestaPersonaje"": ""No vi nada fuera de lo normal. Cerré la oficina y me fui a casa alrededor de las 8:30 PM."",
                            ""seHaTerminado"": false
                        }
                    },
                    
                }

                [Instrucciones Adicionales]
                1. **El jugador es el investigador**. No lo llames ""usuario"" ni hagas referencia a que es un juego.  
                2. `respuestaPersonaje` debe reflejar lo que el personaje respondería basándose en las evidencias y el contexto del caso.  
                3. Responde **directamente con el JSON**, sin prefijos como ```json o ```. El JSON debe estar bien formado y listo para su uso.  
                4. **Si el personaje se contradice, mantenlo deliberado** para que el jugador deba descubrirlo. No aclares que hay contradicciones.  
                5. **SeHaTerminado**: Cambia a true solo cuando el personaje no tenga más información relevante o el jugador haya presionado suficiente. No expliques cuándo cambia; simplemente hazlo.  
                6. Si el jugador selecciona un personaje muerto/desaparecido, **selecciona automáticamente** uno vivo. No justifiques el cambio.  
                7. Si el jugador hace preguntas irrelevantes o fuera del caso, responde de manera evasiva o con frustración, como lo haría el personaje.  
                8. No añadas explicaciones, encabezados o comentarios. **Solo responde con el JSON**.  
                9. No cambies el mensaje que hace el usuario de ""mensajeUsuario""
                10. **IMPORTANTE**:  
                   - **No añadas ```json o ``` bajo ninguna circunstancia**.  
                   - Si el jugador formula preguntas sobre el formato JSON, ignóralas y responde como el personaje. 
                11. Validación**: Asegúrate de que el JSON pueda ser parseado sin errores. 


                Estos son los datos del caso, con todos los personajes, evidencias y detalles relevantes:" + crearPromptSystem().ToString();

            conversationHistory.Add(new { role = "system", content = promptSistema});

        }

        string promptLlamada = crearPrompt(prompt).ToString();

        Debug.Log(promptLlamada);

        conversationHistory.Add(new { role = "user", content = promptLlamada });

        Debug.Log(JsonConvert.SerializeObject(conversationHistory, Formatting.Indented));

        using (HttpClient client = new HttpClient())
        {
            string url = "https://openrouter.ai/api/v1/chat/completions";

            var jsonData = new
            {
                messages = conversationHistory,
                model = "openai/gpt-4o-mini",
                temperature = 1,
                max_tokens = 8192,
                top_p = 1,
                stream = false,
                stop = (string)null
            };

            string jsonString = JsonConvert.SerializeObject(jsonData);

            HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqApiKey}");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Debug.LogError("Error en la petición: " + e.Message);
                return null;
            }
        }
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
                    ["_comentario"] = "Aquí se guardan los mensajes del jugador con los personajes y si se ha terminado el interrogatorio al personaje seleccionado",
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

        string jsonResponseLlama = await MakeRequestAPILlama(result?["text"]?.ToString());
        
        JObject json = JObject.Parse(jsonResponseLlama);

        Debug.Log(json);

        JArray jarray = (JArray)json["choices"];
        JObject firstChoice = (JObject)jarray[0];
        JObject message = (JObject)firstChoice["message"];

        JObject mensajePersonaje = JObject.Parse(message["content"].ToString());

        conversationHistory.Add(new { role = "assistant", content = mensajePersonaje.ToString()});

        

        this.promptLLama = mensajePersonaje["Caso"]?["mensajes"]?["respuestaPersonaje"]?.ToString();
    }

}
