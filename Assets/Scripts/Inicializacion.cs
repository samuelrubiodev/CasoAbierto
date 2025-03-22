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
    public static long jugadorID = 0;
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

        [IMPORTANTE]
        1. EL jugador es el detective, por lo tanto no puede ser el asesino, ni estar involucrado en el crimen, el es el encargado de resolver el caso.
        2. El jugador no debe morir, el es el detective.
        
        Ten en cuenta que el jugador tiene ya los siguientes casos, a si que no se pueden repetir: ";

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

    public async Task crearBaseDatosRedis(long jugadorID)
    {
        Jugador jugador = new ();
        string casosGenerados = "";

        if (jugadorID == -1)
        {
            HashEntry[] hashEntries = Jugador.GetHashEntriesJugador(nombreJugador);
            jugadorID = await Util.GetNewId("jugadores", hashEntries, redisManager);
            Inicializacion.jugadorID = jugadorID;
        }
        else 
        {
            jugador = await Task.Run(() => redisManager.GetPlayer(jugadorID));
            Inicializacion.jugadorID = jugadorID;
            casosGenerados = Caso.AddCasoDetails(jugador);
        }
        
        JObject respuestaCaso = null;
        try
        {
            string respuestaCasoOpenRouter = obtenerRespuestaIA(PROMPT_SYSTEM_GENERACION_CASO + casosGenerados, nombreJugador, apiKeyOpenRouter);
            JObject json = JObject.Parse(respuestaCasoOpenRouter);
            respuestaCaso = json;

            long casoID = await Caso.SetHashCaso(respuestaCaso, jugadorID, redisManager);
            await Task.WhenAll(
                Jugador.SetHashPlayer(respuestaCaso, jugadorID, casoID, redisManager),
                Evidencia.SetHashEvidence(respuestaCaso, jugadorID, casoID,redisManager),
                Cronologia.SetHashTimeline(respuestaCaso, jugadorID, casoID,redisManager)
            );
            
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
        string jsonSchema = GetJsonSchema();

        try
        {
            OpenAIClientOptions openAIClientOptions = new ()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new (new ApiKeyCredential(apiKeyOpenRouter), openAIClientOptions);

            List<ChatMessage> messages = new ()
            {
                new SystemChatMessage(promptSystem),
                new UserChatMessage("Generame un nuevo caso con el nombre de jugador/detective llamado: " + nombreJugador)
            };

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                        jsonSchemaFormatName: "caso_data",
                        jsonSchema: BinaryData.FromString(jsonSchema),
                        jsonSchemaIsStrict: true),
            };

            ChatCompletion completion = client.GetChatClient("google/gemini-2.0-flash-001").CompleteChat(messages, options);

            using JsonDocument jsonDocument = JsonDocument.Parse(completion.Content[0].Text);
            return jsonDocument.RootElement.ToString();
        } 
        catch (Exception e)
        {
            Debug.LogError("Error al parsear el JSON Schema: " + e.Message);
            return null;
        }
    }

    private string GetJsonSchema() {
        return @"
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
                            ""tiempoRestante"": { ""type"": ""string"", ""description"": ""Minutos restantes, solo en formato: MM"" },
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
                                            ""estado_emocional"": {""type"": ""string"", ""description"": ""Nervioso,Tranquilo,Confiado,Arrogante,Asustado,Confuso,Defensivo,Culpable,etc..""},
                                            ""sexo"": {""type"": ""string"", ""description"": ""Masculino o Femenino""}
                                        },
                                        ""required"": [""nombre"",""rol"",""estado"",""descripcion"",""estado_emocional"", ""sexo""],
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
    }
}
