using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using OpenAI.Images;
using System;
using System.IO;

public class ControllerCarga : MonoBehaviour
{
    public static bool tieneCaso = false;

    SQLiteManager sqLiteManager;
    VaultTransit vaultTransit;
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
            SceneManager.LoadScene("SampleScene");
        }
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
        ApiKey.API_KEY_OPEN_ROUTER = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey);
        ApiKey.API_KEY_GROQ = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.GROQ].apiKey);
        ApiKey.API_KEY_ELEVENLABS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.ELEVENLABS].apiKey);

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

    private void GenerateImage(string prompt) {
        Config config = new ("config");
        string togetherApiKey = config.GetKey("TOGETHER_AI");

        ChatManager chatManager = new (togetherApiKey,ChatManager.TOGETHER_AI_API_URL);
        ImageGenerationOptions options = chatManager.CreateImageGenerationOptions(GeneratedImageSize.W1024xH1024, GeneratedImageFormat.Bytes);
        GeneratedImage image = chatManager.GenerateImage(ChatManager.IMAGE_MODEL_FREE, prompt, options);
        BinaryData bytes = image.ImageBytes;
        
        using FileStream stream = File.OpenWrite($"{Application.persistentDataPath}/{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
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
