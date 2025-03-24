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
            string respuestaCasoOpenRouter = obtenerRespuestaIA(PromptSystem.PROMPT_SYSTEM_GENERATION_CASE + casosGenerados, nombreJugador, apiKeyOpenRouter);
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
        string jsonSchema = Schemas.GENERATION_CASE;

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
}
