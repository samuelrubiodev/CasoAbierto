using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private UIDocument uIDocument;

    void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();
    }

    async void Start()
    {
        VisualElement container = uIDocument.rootVisualElement.Q<VisualElement>("game-information");

        await VerPartidas(container, PlayerPrefs.GetInt("jugadorID"));
    }

    async Task VerPartidas(VisualElement container, long jugadorID)
    {
        ScrollView scrollView = container.Q<ScrollView>("game-scroll-view");
        scrollView.Clear();

        var urlBase = "http://" + Server.ACTIVE_CASE_HOST;

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(urlBase + "/players/" + jugadorID);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseBody);
            Jugador jugador = Jugador.FromJSONtoObject(jsonResponse);

            for (int i = 0; i < jugador.casos.Count; i++)
            {
                Caso caso = jugador.casos[i];

                var client = new HttpClient();
                var responseImage = await client.GetAsync(urlBase + "/case/" + caso.idCaso + "/image");
                byte[] imageBytes = await responseImage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                Texture2D texture = new (1, 1);
                texture.LoadImage(imageBytes);
                
                VisualElement containerGame = new ();
                containerGame.AddToClassList("game");

                VisualElement image = new ();
                image.AddToClassList("image");
                image.style.backgroundImage = new StyleBackground(texture);
                
                VisualElement gameContent = new ();
                gameContent.AddToClassList("game-content");

                Label title = new ();
                title.AddToClassList("game-title");
                title.text = "Caso " + (i + 1).ToString() + ": " + caso.tituloCaso;

                Label timePlayed = new ();
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
        }
        else
        {
            Debug.Log($"Request failed with status code: {response.StatusCode}");
        }
        
    }
    public void JugarPartida(Jugador jugador, Caso caso)
    {
        Jugador.jugador = jugador;
        Caso.caso = caso;
        ControllerCarga.tieneCaso = true;
        SceneManager.LoadScene("PantallaCarga");
    }
}