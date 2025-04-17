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
using System.Net.Http;
using Newtonsoft.Json;

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
            //APIRequest.DATOS_CASO = CrearPromptSystem().ToString();
            SceneManager.LoadScene("SampleScene");
            return;
        }
        else
        {
            long jugadorID = GetJugadorID();
            await CreateGame(6);
            /*
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
            
            */
            SceneManager.LoadScene("SampleScene");
        }
    }

    private async Task CreateGame(long playerID)
    {
        var urlBase = "http://" + Server.ACTIVE_CASE_HOST;
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(urlBase);

       var jsonData = new
        {
            nombreJugador = "Samuel",
            playerID = playerID
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(jsonData),
            System.Text.Encoding.UTF8,
            "application/json");

        try 
        {
            var responseNewCase = await httpClient.PostAsync("/case/new", jsonContent);
            string body = await responseNewCase.Content.ReadAsStringAsync().ConfigureAwait(false);

            JObject responseJsonNewCase = JObject.Parse(body);
    
            int caseID = (int)responseJsonNewCase["caseID"];

            var responseCase = await httpClient.PostAsync("/case/" + caseID, jsonContent);
            JObject responseJsonCase = JObject.Parse(await responseCase.Content.ReadAsStringAsync());
            
            Jugador jugador = Jugador.FromJSONtoObject(responseJsonCase);
            Jugador.jugador = jugador;
            Jugador.indexCaso = 0;
            APIRequest.DATOS_CASO = responseJsonCase.ToString();
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"Error: {e.Message}");
        }
    }

    private long GetJugadorID()
    {
        return PlayerPrefs.HasKey("jugadorID") ? PlayerPrefs.GetInt("jugadorID") : -1;
    }
}
