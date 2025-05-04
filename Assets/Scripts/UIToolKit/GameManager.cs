using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utilities.Extensions;

public class GameManager : MonoBehaviour
{
    private UIDocument uIDocument;
    public ErrorOverlayUI errorOverlayUI;   

    void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();
    }

    async void Start()
    {
        await Initializer();
    }

    private async Task Initializer()
    {
        VisualElement container = uIDocument.rootVisualElement.Q<VisualElement>("game-information");

        CallBackButton tryButton = OnTryButtonClicked;
        errorOverlayUI.SetTryCallBackButtonDelegate(tryButton);

        try {
            await VerPartidas(container, PlayerPrefs.GetInt("jugadorID"));
        } catch (Exception) {
            this.SetActive(false);
            UtilitiesErrorMessage errorMessage = new(Application.dataPath + "/Resources/ErrorMessages/ErrorMessage.json");
            ErrorsMessage errors = errorMessage.ReadJSON();

            System.Random random = new();
            int randomTitle = random.Next(0, errors.conexion.titulos.Count);
            int randomMessage = random.Next(0, errors.conexion.mensajes.Count);

            errorOverlayUI.ShowError(errors.conexion.titulos[randomTitle], errors.conexion.mensajes[randomMessage]);
        }
    }

    async Task VerPartidas(VisualElement container, long jugadorID)
    {
        try
        {
            ScrollView scrollView = container.Q<ScrollView>("game-scroll-view");
            scrollView.Clear();

            var urlBase = "http://" + Server.ACTIVE_CASE_HOST;
            CaseHttpRequest caseHttpRequest = new();
            ImageHttpRequest imageHttpRequest = new();

            JObject jsonResponse = await caseHttpRequest.GetAsync("/players/" + jugadorID + "/case");
            Jugador jugador = Jugador.FromJSONtoObject(jsonResponse);

            for (int i = 0; i < jugador.casos.Count; i++)
            {
                try
                {
                    Caso caso = jugador.casos[i];

                    byte[] imageBytes = await imageHttpRequest.GetAsync("/case/" + caso.idCaso + "/image");

                    Texture2D texture = new(1, 1);
                    texture.LoadImage(imageBytes);
                    
                    VisualElement containerGame = new();
                    containerGame.AddToClassList("game");

                    VisualElement image = new();
                    image.AddToClassList("image");
                    image.style.backgroundImage = new StyleBackground(texture);
                    
                    VisualElement gameContent = new();
                    gameContent.AddToClassList("game-content");

                    Label title = new();
                    title.AddToClassList("game-title");
                    title.text = "Caso " + (i + 1).ToString() + ": " + caso.tituloCaso;

                    Label timePlayed = new();
                    timePlayed.AddToClassList("game-time");
                    timePlayed.text = "Tiempo jugado: " + caso.tiempoRestante.ToString();
                    
                    containerGame.Add(image);

                    gameContent.Add(title);
                    gameContent.Add(timePlayed);

                    containerGame.Add(gameContent);

                    containerGame.RegisterCallback<ClickEvent>(ev =>
                    {
                        JugarPartida(jugador, caso);
                    });

                    scrollView.Add(containerGame);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing case {i}: {ex.Message}");
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in VerPartidas: {ex.Message}");
            throw;
        }
    }
    public void JugarPartida(Jugador jugador, Caso caso)
    {
        Jugador.jugador = jugador;
        Caso.caso = caso;
        ControllerCarga.tieneCaso = true;
        SceneManager.LoadScene("PantallaCarga");
    }

    public async void OnTryButtonClicked()
    {
        this.SetActive(true);
        await Initializer();
    }
}