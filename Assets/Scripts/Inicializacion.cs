using GroqApiLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using UnityEngine;
using Utilities.WebRequestRest;

public class Inicializacion
{
    public const string PROMPT_SYSTEM_GENERACION_CASO = @"
        [Contexto del Juego]
        Estás desarrollando un juego de investigación policial llamado ""Caso Abierto"". El jugador asume el rol de un detective encargado de resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias. El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

        [Objetivo de la IA]
        Tu tarea es generar un archivo JSON que defina los detalles de un caso de investigación. Responde directamente con el JSON, sin añadir texto adicional ni marcas como json o.

        [IMPORTANTE]
        NO AÑADAS ```json o ``` BAJO NINGUNA CIRCUNSTANCIA.
        SOLO el JSON puro, sin formato adicional.  
        Es extremadamente importante que la respuesta NO tenga marcas de código.  
        SI se añaden accidentalmente, genera el JSON nuevamente desde cero.  
        Esta instrucción es prioritaria y se repetirá para asegurarte de que la respuesta es correcta.


        [Creatividad y Realismo]
        Los casos deben ser realistas, variados y originales, pero siempre dentro de los límites de lo posible en un entorno policial o criminalístico. Evita cualquier elemento de ciencia ficción, paranormal o sobrenatural. Los misterios deben resolverse con lógica, deducción y evidencia.

        [Estructura del JSON]
        El JSON debe incluir:

        datosJugador: Información sobre el jugador (estado, progreso, nombre).
        Caso: Detalles del caso (título, descripción, fecha, lugar, tiempo restante).
        cronologia: Lista de eventos con fecha, hora y descripción (mínimo 2).
        evidencias: Hasta 5 evidencias con nombre, descripción, análisis, tipo y ubicación.
        personajes: Lista de personajes clave con nombre, rol, estado, descripción y estado emocional.
        explicacionCasoResuelto: Descripción de cómo se resuelve el caso.
        
        [Instrucciones Adicionales]

        1. Los casos deben ser variados y originales. Evita repetir tramas como asesinatos en oficinas, robos en museos o crímenes domésticos. 
        2. Las tramas deben explorar distintos tipos de delitos plausibles: fraudes financieros, extorsión, secuestros, tráfico de arte, desapariciones, estafas, corrupción o espionaje industrial.
        3. Evita cualquier explicación que involucre artefactos misteriosos, alucinaciones inexplicables o elementos paranormales.
        4. El JSON NO debe contener delimitadores como ```json o ```.    
        5. Inspírate en casos reales o crímenes complejos que requieran análisis detallado. Los giros argumentales deben basarse en evidencia forense, testimonios y contradicciones de los sospechosos.
        6. Obliga a que al menos uno de los personajes tenga un secreto o un motivo oculto que no sea evidente a simple vista.
        7. Las evidencias deben ser variadas: huellas digitales, grabaciones de cámaras, registros telefónicos, documentos, armas, testimonios contradictorios o pruebas forenses.
        8. NO añadas explicaciones, encabezados o comentarios. Solo responde con el JSON directamente.  
        9. Repite: No uses ```json ni ``` en ningún momento. Solo el JSON puro.        
        10. Si no hay evidencias o personajes disponibles, deja el array vacío ""[]"".
        11. Asegúrate de que los eventos y personajes sean coherentes con el caso descrito.
        12. No añadas comentarios, encabezados o explicaciones en la respuesta final. Solo responde con el JSON.
        13. ES MUY IMPORTANTE QUE NO AÑADAS ```json o ```, al principio o al final del JSON. Solo el JSON en si.";


    public string nombreJugador;
    private SQLiteManager sqLiteManager;
    public Inicializacion(string nombreJugador)
    {
        this.nombreJugador = nombreJugador;
        sqLiteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqLiteManager.crearConexion();
    }

    public async Task crearBaseDatosSQLite(string servicio, string apiKey)
    {
        var vaultTransit = new VaultTransit();

        sqLiteManager.CreateTable<ApiKey>();

        string apiKey_cifrada = await vaultTransit.EncryptAsync("api-key-encrypt", apiKey);

        var objetoApiKey = new ApiKey { 
            name = servicio,
            apiKey = apiKey_cifrada
        };

        sqLiteManager.Insert(objetoApiKey);
    }

    public async Task crearBaseDatosRedis(string ip, string port, string user, string password)
    {
        RedisManager redisManager = new RedisManager(ip,port,password);
        redisManager.crearConexion();

        HashEntry[] hashEntries = new HashEntry[]
        {
            new HashEntry("nombre", nombreJugador),
            new HashEntry("estado", "inactivo"),
            new HashEntry ("progreso","SinCaso"),
            new HashEntry("ultima_conexion", DateTime.Now.ToString())
        };

        long jugadorID = redisManager.GetNewId("jugadores");
        redisManager.SetHash($"jugadores:{jugadorID}", hashEntries);

        VaultTransit vaultTransit = new VaultTransit();

        string apiKeyGroq = "";

        sqLiteManager.CreateTable<Player>();

        var player = new Player
        {
            idPlayer = jugadorID,
        };

        sqLiteManager.Insert(player);

        foreach (ApiKey apiKey in sqLiteManager.GetTable<ApiKey>("SELECT * FROM ApiKeys"))
        {
            if (apiKey.name == "OpenRouter")
            {
                apiKeyGroq = await vaultTransit.DecryptAsync("api-key-encrypt", apiKey.apiKey);
                break;
            }
        }

        MessageManager messageManager = new MessageManager();

        messageManager.AddMessage("system", PROMPT_SYSTEM_GENERACION_CASO + "\n" + enviarPromptSystemGeneracionCaso(nombreJugador).ToString());

        GroqApiClient groqApiClient = new GroqApiClient(apiKeyGroq, "https://openrouter.ai/api/v1");

        JObject respuesta = await groqApiClient.CreateChatCompletionAsync(messageManager.AgregarRequest("openai/gpt-4o-mini", 1, 8192));
        
        Debug.Log(respuesta.ToString());

        string respuestaCasoString = respuesta?["choices"]?[0]?["message"]?["content"]?.ToString();
        Debug.Log(respuestaCasoString);
        JObject respuestaCaso = null;

        try
        {
            respuestaCaso = JObject.Parse(respuestaCasoString);
        }
        catch (JsonReaderException ex)
        {
            Debug.Log("JSON mal formado, solicitando corrección a la IA...");
            respuestaCaso = await ArreglarJSON(respuestaCasoString, groqApiClient, apiKeyGroq);
            return;
        }

        foreach (JObject personaje in respuestaCaso["Caso"]?["personajes"])
        {
            HashEntry[] hashPersonajes = new HashEntry[]
            {
                new HashEntry("nombre", personaje["nombre"].ToString()),
                new HashEntry("rol", personaje["rol"].ToString()),
                new HashEntry ("descripcion",personaje["descripcion"].ToString()),
                new HashEntry("estado", personaje["estado"].ToString()),
                new HashEntry("estado_emocional", personaje["estado_emocional"].ToString())
            };

            long personajeID = redisManager.GetNewId($"jugadores:{jugadorID}:personajes");
            redisManager.SetHash($"jugadores:{jugadorID}:personajes:{personajeID}", hashPersonajes);
        }

        foreach (JObject evidencia in respuestaCaso["Caso"]?["evidencias"])
        {
            HashEntry[] hashEvidencias = new HashEntry[]
            {
                new HashEntry("nombre", evidencia["nombre"].ToString()),
                new HashEntry("descripcion", evidencia["descripcion"].ToString()),
                new HashEntry("analisis", evidencia["analisis"].ToString()),
                new HashEntry("tipo", evidencia["tipo"].ToString()),
                new HashEntry("ubicacion", evidencia["ubicacion"].ToString())
            };

            long evidenciaID = redisManager.GetNewId($"jugadores:{jugadorID}:evidencias");
            redisManager.SetHash($"jugadores:{jugadorID}:evidencias:{evidenciaID}", hashEvidencias);
        }

        HashEntry[] hashCaso = new HashEntry[]
        {
            new HashEntry("tituloCaso", respuestaCaso["Caso"]["tituloCaso"].ToString()),
            new HashEntry("descripcionCaso", respuestaCaso["Caso"]["descripcionCaso"].ToString()),
            new HashEntry("dificultad", respuestaCaso["Caso"]["dificultad"].ToString()),
            new HashEntry("fechaOcurrido", respuestaCaso["Caso"]["fechaOcurrido"].ToString()),
            new HashEntry("lugar", respuestaCaso["Caso"]["lugar"].ToString()),
            new HashEntry("tiempoRestante", respuestaCaso["Caso"]["tiempoRestante"].ToString()),
            new HashEntry("explicacionCasoResuelto", respuestaCaso["Caso"]["explicacionCasoResuelto"].ToString())
        };

        long casoID = redisManager.GetNewId($"jugadores:{jugadorID}:caso");
        redisManager.SetHash($"jugadores:{jugadorID}:caso:{casoID}", hashCaso);

        foreach (JObject cronologia in respuestaCaso["Caso"]["cronologia"])
        {
            HashEntry[] hashCronologia = new HashEntry[]
            {
                new HashEntry("fecha", cronologia["fecha"].ToString()),
                new HashEntry("hora", cronologia["hora"].ToString()),
                new HashEntry("evento", cronologia["evento"].ToString())
            };

            long cronologiaID = redisManager.GetNewId($"jugadores:{jugadorID}:caso:{casoID}:cronologia");
            redisManager.SetHash($"jugadores:{jugadorID}:caso:{casoID}:cronologia:{cronologiaID}", hashCronologia);
        }

    }
    
    private JObject enviarPromptSystemGeneracionCaso(string nombreJugador) 
    { 
        return new JObject
        {
            ["datosJugador"] = new JObject
            {
                ["_comentario"] = "Datos importantes del jugador, no cambies el nombre del jugador",
                ["_estado"] = "Activo o Inactivo",
                ["_progreso"] = "En que caso va, poner nombre del caso",
                ["nombre"] = nombreJugador,
                ["estado"] = "",
                ["progreso"] = ""
            },
            ["Caso"] = new JObject
            {
                ["_comentario"] = "Datos del caso actual",
                ["tituloCaso"] = "",
                ["descripcionCaso"] = "",
                ["dificultad"] = "Facil, Medio y Dificil",
                ["fechaOcurrido"] = "YYYY-MM-DD",
                ["lugar"] = "",
                ["tiempoRestante"] = "",

                ["cronologia"] = new JArray
                {
                    new JObject
                    {
                        ["fecha"] = "YYYY-MM-DD",
                        ["hora"] = "HH:MM",
                        ["evento"] = "Descripcion breve del evento"
                    },
                    new JObject
                    {
                        ["fecha"] = "YYYY-MM-DD",
                        ["hora"] = "HH:MM",
                        ["evento"] = "Otro evento relevante del caso"
                    }
                },

                ["evidencias"] = new JArray
                {
                    new JObject
                    {
                        ["_comentario"] = "Estos son ejemplos, no los copies",
                        ["nombre"] = "Nombre de la evidencia",
                        ["descripcion"] = "Una carta manchada de sangre, un cuchillo con huellas dactilares, una foto de la víctima con un mensaje amenazante, un diario con una página arrancada, etc..",
                        ["analisis"] = "Un análisis de la evidencia, puede ser una descripción de lo que se encontró, una conclusión de lo que significa, etc..",
                        ["tipo"] = "Arma, Documento, Objeto personal, Foto, Video, etc..",
                        ["ubicacion"] = "Donde se encontro la evidencia"
                    }
                },
                ["personajes"] = new JArray
                {
                    new JObject
                    {
                        ["_comentario"] = "Estos son ejemplos, no los copies",
                        ["nombre"] = "Nombre del personaje",
                        ["rol"] = "Testigo, Victima, Cómplice, Informante, Periodista, Familia del sospechoso, etc..",
                        ["estado"] = "Vivo, Muerto o Desaparecido",
                        ["descripcion"] = "Un hombre mayor con un aire autoritario y una cicatriz prominente en la mejilla. Una mujer joven con gafas grandes y un nerviosismo evidente al hablar. Una ancina amable pero con un comportamiento claramente evasivo. Una joven madre que abraza una foto familiar mientras habla contigo, etc..",
                        ["estado_emocional"] = "Nervioso,Tranquilo,Confiado,Arrogante,Asustado,Confuso,Defensivo,Culpable,etc.."
                    }
                },
                ["explicacionCasoResuelto"] = "Explicación de como se resuelve el caso"
            },
        };
    }

    private async Task<JObject> ArreglarJSON(string rawJson, GroqApiClient groqApiClient, string apiKeyGroq)
    {
        try
        {
            return JObject.Parse(rawJson);
        }
        catch (JsonReaderException ex)
        {
            Debug.LogWarning($"Error de formato JSON: {ex.Message}");
            Debug.Log("Solicitando corrección del JSON a la IA...");

            string promptCorreccion = $@"
                He recibido un JSON malformado. Por favor, corrige cualquier error de sintaxis, como comas adicionales,
                propiedades sin valores o claves mal escritas. Tienes que devolver SOLAMENTE el json, no añadas ningun comentario tuyo ni nada parecido, tampoco pongas ```json ni ```
                Aquí está el JSON recibido: {rawJson}";

            MessageManager messageManager = new();
            messageManager.AddMessage("system", promptCorreccion);

            JObject respuesta = await groqApiClient.CreateChatCompletionAsync(
                messageManager.AgregarRequest("llama-3.3-70b-specdec", 1, 8192)
            );

            string contenidoCorregido = respuesta?["choices"]?[0]?["message"]?["content"]?.ToString();

            try
            {
                return JObject.Parse(contenidoCorregido);
            }
            catch (Exception innerEx)
            {
                Debug.LogError($"Error crítico: El JSON corregido aún no es válido. {innerEx.Message}");
                throw;
            }
        }
    }
}
