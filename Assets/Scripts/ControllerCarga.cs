using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using OpenAI.Images;
using System;
using System.IO;
using OpenAI.Chat;
using TMPro;
using System.Collections;

public class ControllerCarga : MonoBehaviour
{
    public static bool tieneCaso = false;
    private BinaryData bytes;
    public TMP_Text text;

    private int currentSubtitleIndex;
    private bool isShowingMessage = false;
    private UIMessageManager uiMessageManager;
    private SubtitleList subtitulos;

    private float messageTimer = 0f;
    private const float messageDelay = 2f;

    void Update()
    {
        if (!isShowingMessage)
        {
            messageTimer += Time.deltaTime;
            if (messageTimer >= messageDelay)
            {
                messageTimer = 0f;
                ShowNextMessage();
            }
        }
    }

    private void ShowNextMessage()
    {
        isShowingMessage = true;
        currentSubtitleIndex = (currentSubtitleIndex + 1) % subtitulos.subtitles.Count;
        StartCoroutine(ShowMessageAndWaitForCompletion());
    }

    private IEnumerator ShowMessageAndWaitForCompletion()
    {
        yield return StartCoroutine(uiMessageManager.ShowMessage(uiMessageManager.GetMessage(subtitulos.subtitles[currentSubtitleIndex].text)));
        isShowingMessage = false;
    }
    async void Start()
    {
        new Recomendations(text).SetStyle();
        uiMessageManager = new(text);
        subtitulos = new GenericSubtitles(SubtitlesPath.TIPS_RESEARCH).ReturnSubtitleList();
        currentSubtitleIndex = UnityEngine.Random.Range(0, subtitulos.subtitles.Count);
        ShowNextMessage();

        if (tieneCaso)
        {
            APIRequest.DATOS_CASO = CrearPromptSystem().ToString();
            SceneManager.LoadScene("SampleScene");
            return;
        }
        else
        {
            RedisManager redisManger = RedisManager.GetRedisManagerEnv();
            long jugadorID = GetJugadorID();

            await CrearCaso(redisManger, ApiKey.API_KEY_OPEN_ROUTER,jugadorID);
            
            if (jugadorID == -1)
            {
                PlayerPrefs.SetInt("jugadorID", (int)Inicializacion.jugadorID);
                PlayerPrefs.Save();

                jugadorID = Inicializacion.jugadorID;
            }  

            Debug.Log($"jugadorID: {jugadorID}");

            await CargarPartida(redisManger, jugadorID);
            SaveImage(redisManger);
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void SaveImage(RedisManager redisManager)
    {
        using FileStream stream = File.OpenWrite($"{Application.persistentDataPath}/{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);
        byte[] imageBytes = bytes.ToArray();
        redisManager.SaveImage($"jugadores:{Jugador.jugador.idJugador}:caso:{Inicializacion.idCasoGenerado}:imagen", imageBytes);
    }

    async Task CargarPartida(RedisManager redisManager, long jugadorID)
    {
        Jugador jugador = await Task.Run(() =>
        {
            return redisManager.GetPlayer(jugadorID);
        });

        Debug.Log($"ID Caso Generado: {Inicializacion.idCasoGenerado}");
        
        for(int i = 0; i < jugador.casos.Count; i++)
        {
            if(jugador.casos[i].idCaso == Inicializacion.idCasoGenerado.ToString())
            {
                Jugador.jugador = jugador;
                Jugador.indexCaso = i;
                Debug.Log($"ID Caso: {i}");
                break;
            }
        }

        await Task.Run(async () => {
            string prompt = await CreatePromptForImage(Jugador.jugador.casos[Jugador.indexCaso]);
            await GenerateImage(prompt);
        });
        
        APIRequest.DATOS_CASO = CrearPromptSystem().ToString();
    }
    
    async Task CrearCaso(RedisManager redisManager, string apiKeyOpenRouter, long jugadorID)
    {
        await Task.Run(async () =>
        {
            Inicializacion inicializacion = new("Samuel");

            inicializacion.setRedisManager(redisManager);
            inicializacion.setApiKeyOpenRouter(apiKeyOpenRouter);

            await inicializacion.crearBaseDatosRedis(jugadorID);
        });
    }

    private long GetJugadorID()
    {
        return PlayerPrefs.HasKey("jugadorID") ? PlayerPrefs.GetInt("jugadorID") : -1;
    }

    private async Task<string> CreatePromptForImage(Caso caso)
    {
        List<ChatMessage> messages = new() 
        {
            new SystemChatMessage(PromptSystem.PROMPT_SYSTEM_IMAGE_GENERATION),
            new UserChatMessage("Generate a prompt based on this case: " + caso.ToString())
        };
        ChatManager chatManager = new (ApiKey.API_KEY_OPEN_ROUTER,messages);
        return await chatManager.SendMessageAsync(ChatManager.CHAT_MODEL);
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
