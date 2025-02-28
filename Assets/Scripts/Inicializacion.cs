using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using OpenAI;
using StackExchange.Redis;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.Json;

public class Inicializacion
{

    public static int idCasoGenerado = 0;
    public const string PROMPT_SYSTEM_GENERACION_CASO = @"
        [Contexto del Juego]
        Estás desarrollando un juego de investigación policial llamado ""Caso Abierto"".
        El jugador asume el rol de un detective, el se dedicará a resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias.
        El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

        [Objetivo de la IA]
        Definir los detalles de un caso de investigación

        [Creatividad y Realismo]
        Los casos deben ser realistas, variados y originales, pero siempre dentro de los límites de lo posible en un entorno policial o criminalístico. Evita cualquier elemento de ciencia ficción, paranormal o sobrenatural. Los misterios deben resolverse con lógica, deducción y evidencia.
        
        [Instrucciones importantes
        1. Los casos deben ser variados y originales. Evita repetir tramas como asesinatos en oficinas, robos en museos o crímenes domésticos. 
        2. Las tramas deben explorar distintos tipos de delitos plausibles: fraudes financieros, extorsión, secuestros, tráfico de arte, desapariciones, estafas, corrupción o espionaje industrial.
        3. Evita cualquier explicación que involucre artefactos misteriosos, alucinaciones inexplicables o elementos paranormales.
        4. Inspírate en casos reales o crímenes complejos que requieran análisis detallado. Los giros argumentales deben basarse en evidencia forense, testimonios y contradicciones de los sospechosos.
        5. Obliga a que al menos uno de los personajes tenga un secreto o un motivo oculto que no sea evidente a simple vista.
        6. Las evidencias deben ser variadas: huellas digitales, grabaciones de cámaras, registros telefónicos, documentos, armas, testimonios contradictorios o pruebas forenses.     
        7. Asegúrate de que los eventos y personajes sean coherentes con el caso descrito.
        8. El jugador no debe morir, el es el detective";

    public string nombreJugador;
    private SQLiteManager sqLiteManager;
    private RedisManager redisManager;
    private VaultTransit vaultTransit;
    private string apiKeyOpenRouter;
    public Inicializacion(string nombreJugador)
    {
        this.nombreJugador = nombreJugador;
    }

    public void setSQliteManager(SQLiteManager sqLiteManager)
    {
        this.sqLiteManager = sqLiteManager;
    }

    public void setRedisManager(RedisManager redisManager)
    {
        this.redisManager = redisManager;
    }

    public void setVaultTransit(VaultTransit vaultTransit)
    {
        this.vaultTransit = vaultTransit;
    }

    public void setApiKeyOpenRouter(string apiKeyOpenRouter)
    {
        this.apiKeyOpenRouter = apiKeyOpenRouter;
    }

    public async Task crearBaseDatosRedis()
    {
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

        string apiKeyGroq = "";

        sqLiteManager.CreateTable<Player>();
        var player = new Player
        {
            idPlayer = jugadorID,
        };
        sqLiteManager.Insert(player);

        apiKeyGroq = apiKeyOpenRouter;

        JObject respuestaCaso = null;

        try
        {
            string respuestaCasoOpenRouter = obtenerRespuestaIA(PROMPT_SYSTEM_GENERACION_CASO, nombreJugador, apiKeyGroq);
            JObject json = JObject.Parse(respuestaCasoOpenRouter);
            respuestaCaso = json;

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

                long personajeID = redisManager.GetNewId($"jugadores:{jugadorID}:caso:{casoID}:personajes");
                redisManager.SetHash($"jugadores:{jugadorID}:caso:{casoID}:personajes:{personajeID}", hashPersonajes);
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

                long evidenciaID = redisManager.GetNewId($"jugadores:{jugadorID}:caso:{casoID}:evidencias");
                redisManager.SetHash($"jugadores:{jugadorID}:caso:{casoID}:evidencias:{evidenciaID}", hashEvidencias);
            }
            
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
            idCasoGenerado = (int)casoID;
        }
        catch (JsonReaderException ex)
        {
            Debug.LogError("Error al parsear la respuesta de OpenRouter: " + ex.Message);
            return;
        }
    }

    public string obtenerRespuestaIA(string promptSystem,string nombreJugador, string apiKeyOpenRouter)
    {
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
                            ""dificultad"": { ""type"": ""string"", ""description"": ""Facil, Medio o Dificil"" },
                            ""fechaOcurrido"": { ""type"": ""string"", ""description"": ""YYYY-MM-DD"" },
                            ""lugar"": { ""type"": ""string"", ""description"": ""Lugar en el que ha ocurrido el caso"" },
                            ""tiempoRestante"": { ""type"": ""string"", ""description"": ""HH:MM"" },
                            ""cronologia"": { ""type"": ""array"", ""items"": {
                                    ""type"": ""object"",
                                        ""properties"": {
                                            ""fecha"": {""type"": ""string"", ""description"": ""YYYY-MM-DD""},
                                            ""hora"": {""type"": ""string"", ""description"": ""HH:MM""},
                                            ""evento"": {""type"": ""string"", ""description"": ""Descripcion breve del evento""}
                                        },
                                        ""required"": [""fecha"",""hora"",""evento""],
                                        ""additionalProperties"": false
                                    } 
                            },
                            ""evidencias"": {""type"": ""array"", ""items"": {
                                        ""type"": ""object"",
                                        ""properties"": {
                                            ""nombre"": {""type"": ""string"", ""description"": ""Nombre de la evidencia, objeto""},
                                            ""descripcion"": {""type"": ""string"", ""description"": ""Una carta manchada de sangre, un cuchillo con huellas dactilares, una foto de la víctima con un mensaje amenazante, un diario con una página arrancada, etc..""},
                                            ""analisis"": {""type"": ""string"", ""description"": ""Un análisis de la evidencia, puede ser una descripción de lo que se encontró, una conclusión de lo que significa, etc..""},
                                            ""tipo"": {""type"": ""string"", ""description"": ""Arma, Documento, Objeto personal, Foto, Video, etc..""},
                                            ""ubicacion"": {""type"": ""string""}
                                        },
                                        ""required"": [""nombre"",""descripcion"",""analisis"",""tipo"",""ubicacion""],
                                        ""additionalProperties"": false
                                    }
                            },
                            ""personajes"": {""type"": ""array"", ""items"": {
                                        ""type"": ""object"",
                                        ""properties"": {
                                            ""nombre"": {""type"": ""string"", ""description"": ""Nombre del personaje""},
                                            ""rol"": {""type"": ""string"", ""description"": ""Testigo, Victima, Cómplice, Informante, Periodista, Familia del sospechoso, etc..""},
                                            ""estado"": {""type"": ""string"", ""description"": ""Vivo, Muerto o Desaparecido""},
                                            ""descripcion"": {""type"":""string"", ""description"": ""Ejemplo: Un hombre mayor con un aire autoritario y una cicatriz prominente en la mejilla. Una mujer joven con gafas grandes y un nerviosismo evidente al hablar. Una ancina amable pero con un comportamiento claramente evasivo. Una joven madre que abraza una foto familiar mientras habla contigo, etc..""},
                                            ""estado_emocional"": {""type"": ""string"", ""description"": ""Nervioso,Tranquilo,Confiado,Arrogante,Asustado,Confuso,Defensivo,Culpable,etc..""}
                                        },
                                        ""required"": [""nombre"",""rol"",""estado"",""descripcion"",""estado_emocional""],
                                        ""additionalProperties"": false
                                    }
                            },
                            ""explicacionCasoResuelto"": {""type"": ""string"", ""description"": ""Descripcion de como se podría resolver el caso""}
                        },
                        ""required"": [""tituloCaso"",""descripcionCaso"",""dificultad"",""fechaOcurrido"",""lugar"",""tiempoRestante"",""cronologia"",""evidencias"",""personajes"",""explicacionCasoResuelto""],
                        ""additionalProperties"": false
                    }
                },
                ""required"": [""datosJugador"",""Caso""],
                ""additionalProperties"": false
            }";

        try
        {
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new OpenAIClient(new ApiKeyCredential(apiKeyOpenRouter), openAIClientOptions);

            List<ChatMessage> messages = new List<ChatMessage>
            {
                new SystemChatMessage(promptSystem),
                new UserChatMessage("Generame un nuevo caso con el nombre de jugador llamado: " + nombreJugador)
            };

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                        jsonSchemaFormatName: "caso_data",
                        jsonSchema: BinaryData.FromString(jsonSchema),
                        jsonSchemaIsStrict: true),
            };

            ChatCompletion completion = client.GetChatClient("google/gemini-2.0-flash-exp:free").CompleteChat(messages, options);

            using JsonDocument jsonDocument = JsonDocument.Parse(completion.Content[0].Text);

            return jsonDocument.RootElement.ToString();
        } 
        catch (Exception e)
        {
            Debug.LogError("Error al parsear el JSON Schema: " + e.Message);
            return null;
        }
       
    }
}
