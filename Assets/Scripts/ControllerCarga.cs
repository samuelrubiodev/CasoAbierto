using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using OpenAI.Images;
using System;
using System.IO;
using OpenAI.Chat;

public class ControllerCarga : MonoBehaviour
{
    public static bool tieneCaso = false;

    SQLiteManager sqLiteManager;
    VaultTransit vaultTransit;
    private BinaryData bytes;
    async void Start()
    {
        sqLiteManager = SQLiteManager.GetSQLiteManager();
        vaultTransit = new ();

        await ConectarApis();

        if (tieneCaso)
        {
            APIRequest.DATOS_CASO = CrearPromptSystem().ToString();
            SceneManager.LoadScene("SampleScene");
            return;
        }
        else
        {
            RedisManager redisManger = await RedisManager.GetRedisManager();
            long jugadorID = GetJugadorID();

            await CrearCaso(redisManger, vaultTransit, ApiKey.API_KEY_OPEN_ROUTER,jugadorID);
            
            if (jugadorID == -1)
            {
                PlayerPrefs.SetInt("jugadorID", (int)Inicializacion.jugadorID);
                PlayerPrefs.Save();

                jugadorID = Inicializacion.jugadorID;
            }  

            await CargarPartida(redisManger, jugadorID);
            SaveImage();
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void SaveImage()
    {
        using FileStream stream = File.OpenWrite($"{Application.persistentDataPath}/{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
    }

    async Task CargarPartida(RedisManager redisManager, long jugadorID)
    {
        Jugador jugador = await Task.Run(() =>
        {
            return redisManager.GetPlayer(jugadorID);
        });
        
        for(int i = 0; i < jugador.casos.Count; i++)
        {
            if(jugador.casos[i].idCaso == Inicializacion.idCasoGenerado.ToString())
            {
                Jugador.jugador = jugador;
                Jugador.indexCaso = i;
                break;
            }
        }

        await Task.Run(async () => {
            string prompt = await CreatePromptForImage(Jugador.jugador.casos[Jugador.indexCaso]);
            await GenerateImage(prompt);
        });
        
        APIRequest.DATOS_CASO = CrearPromptSystem().ToString();
    }
    
    async Task CrearCaso(RedisManager redisManager, VaultTransit vaultTransit, string apiKeyOpenRouter, long jugadorID)
    {
        await Task.Run(async () =>
        {
            Inicializacion inicializacion = new("Samuel");

            inicializacion.setSQliteManager(sqLiteManager);
            inicializacion.setRedisManager(redisManager);
            inicializacion.setVaultTransit(vaultTransit);
            inicializacion.setApiKeyOpenRouter(apiKeyOpenRouter);

            await inicializacion.crearBaseDatosRedis(jugadorID);
        });
    }

    async Task ConectarApis()
    {  
        Config config = new ("config");
        ApiKey.API_KEY_OPEN_ROUTER = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey);
        ApiKey.API_KEY_GROQ = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.GROQ].apiKey);
        ApiKey.API_KEY_ELEVENLABS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.ELEVENLABS].apiKey);
        ApiKey.API_KEY_TOGETHER = config.GetKey("TOGETHER_AI");

        Server.IP_SERVER_REDIS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].ipServer);
        Server.CONTRASENA_REDIS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].password);
    }

    private long GetJugadorID()
    {
        if (PlayerPrefs.HasKey("jugadorID"))
            return PlayerPrefs.GetInt("jugadorID");
        else
            return -1;
    }

    private async Task<string> CreatePromptForImage(Caso caso)
    {
        List<ChatMessage> messages = new() 
        {
            new SystemChatMessage(APIRequest.IMAGE_GENERATION_PROMPT_SYSTEM),
            new UserChatMessage("Generate a prompt based on this case: " + caso.ToString())
        };
        ChatManager chatManager = new (ApiKey.API_KEY_OPEN_ROUTER,messages);
        return await chatManager.SendMessageAsync(ChatManager.CHAT_MODEL_FREE);
    }

    private async Task GenerateImage(string prompt) {
        ChatManager chatManager = new (ApiKey.API_KEY_TOGETHER,ChatManager.TOGETHER_AI_API_URL);
        ImageGenerationOptions options = chatManager.CreateImageGenerationOptions(GeneratedImageSize.W1024xH1024, GeneratedImageFormat.Bytes);

        GeneratedImage image = await chatManager.GetImageAsync(ChatManager.IMAGE_MODEL_FREE, prompt, options);
        
        bytes = image.ImageBytes;
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

                ["cronologia"] = Util.ObtenerCronologias(caso.cronologia),
                ["evidencias"] = Util.ObtenerEvidencias(caso.evidencias),
                ["personajes"] = Util.ObtenerPersonajes(caso.personajes),
                ["explicacionCasoResuelto"] = caso.explicacionCasoResuelto
            }
        };

        return objetoJson;
    }
}
