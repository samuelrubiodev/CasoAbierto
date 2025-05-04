using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System;
using TMPro;
using System.Collections;
using System.Net.Http;
using Newtonsoft.Json;
using Utilities.Extensions;

public class ControllerCarga : MonoBehaviour
{
    public static bool tieneCaso = false;
    public TMP_Text text;
    public TMP_Text loadingText;

    private int currentSubtitleIndex;
    private bool isShowingMessage = false;
    private UIMessageManager uiMessageManager;
    private SubtitleList subtitulos;

    private float messageTimer = 0f;
    private const float messageDelay = 2f;
    public ErrorOverlayUI errorOverlayUI;   
    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

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

    private IEnumerator FadeOutCR()
    {
        while (true)
        {
            float currentTime = 0f;
            while (currentTime < fadeInTime)
            {
                float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
                loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            currentTime = 0f;
            while (currentTime < fadeOutTime)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutTime);
                loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
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
        StartCoroutine(FadeOutCR());
        await Initializer();
    }

    private async Task Initializer()
    {
        new Recomendations(text).SetStyle();
        uiMessageManager = new(text);
        subtitulos = new GenericSubtitles(SubtitlesPath.TIPS_RESEARCH).ReturnSubtitleList();
        currentSubtitleIndex = UnityEngine.Random.Range(0, subtitulos.subtitles.Count);
        ShowNextMessage();

        CallBackButton tryButton = OnTryButtonClicked;
        errorOverlayUI.SetTryCallBackButtonDelegate(tryButton);

        if (tieneCaso)
        {
            try {
                CaseHttpRequest caseHttpRequest = new ();
                var jsonData = new
                {
                    nombreJugador = "Samuel",
                    playerID = GetJugadorID()
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(jsonData),
                    System.Text.Encoding.UTF8,
                    "application/json");
                JObject responseCase = await caseHttpRequest.PostAsync("/case/" + Caso.caso.idCaso, jsonContent);
                APIRequest.DATOS_CASO = responseCase.ToString();
                SceneManager.LoadScene("InterrogationGame"); // Or SampleScene (if you not have QA_InterrogationRoom asset)
            } catch (Exception) {
                this.SetActive(false);
                UtilitiesErrorMessage errorMessage = new(Application.dataPath + "/Resources/ErrorMessages/ErrorMessage.json");
                ErrorsMessage errors = errorMessage.ReadJSON();

                System.Random random = new ();
                int randomTitle = random.Next(0, errors.conexion.titulos.Count);
                int randomMessage = random.Next(0, errors.conexion.mensajes.Count);

                errorOverlayUI.ShowError(errors.conexion.titulos[randomTitle], errors.conexion.mensajes[randomMessage]);
            }
            
            return;
        }
        else
        {
            long jugadorID = GetJugadorID();
            if (jugadorID == -1)
            {
                int id = await NewID(jugadorID);
                PlayerPrefs.SetInt("jugadorID", id);
                PlayerPrefs.Save();
            }
            
            await CreateGame(GetJugadorID());
            SceneManager.LoadScene("InterrogationGame"); // Or SampleScene (if you not have QA_InterrogationRoom asset)
        }
    }

    private async void OnTryButtonClicked()
    {
        this.SetActive(true);
        await Initializer();
    }

    private async Task<int> NewID(long playerID)
    {
        if (playerID == -1)
        {
            CaseHttpRequest caseHttpRequest = new ();

            var jsonData = new
            {
                playerName = "Scott Shelby"
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(jsonData),
                System.Text.Encoding.UTF8,
                "application/json");

            JObject jsonResponse = await caseHttpRequest.PostAsync("/players/new", jsonContent);

            JObject player = (JObject)jsonResponse["player"];
            int newPlayerID = (int)player["id"];

            return newPlayerID;
        }
        return -1;
    }

    private async Task CreateGame(long playerID)
    {
        CaseHttpRequest caseHttpRequest = new ();
        var jsonData = new
        {
            nombreJugador = "Samuel",
            playerID = playerID
        };

        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(jsonData),
            System.Text.Encoding.UTF8,
            "application/json");

        JObject jsonResponse = await caseHttpRequest.PostAsync("/case/new", jsonContent);
        int caseID = (int)jsonResponse["caseID"];

        JObject responseCase = await caseHttpRequest.PostAsync("/case/" + caseID, jsonContent);
        Jugador jugador = Jugador.FromJSONtoObject(responseCase);
        LoadCase(jugador, caseID, responseCase);
        APIRequest.DATOS_CASO = responseCase.ToString();
    }

    private void LoadCase(Jugador jugador, int caseID, JObject json)
    {
        Jugador.jugador = jugador;
        for (int i = 0; i < jugador.casos.Count; i++)
        {
            int subCasoID = int.Parse(jugador.casos[i].idCaso);
            if (subCasoID == caseID)
            {
                Caso.caso = jugador.casos[i];
                break;
            }
        }

        APIRequest.DATOS_CASO = json.ToString();
    }

    private long GetJugadorID()
    {
        return PlayerPrefs.HasKey("jugadorID") ? PlayerPrefs.GetInt("jugadorID") : -1;
    }
}
