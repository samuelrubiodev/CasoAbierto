using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private UIDocument uIDocument;
    private RedisManager redisManager;

    void OnEnable()
    {
        uIDocument = GetComponent<UIDocument>();
    }

    async void Start()
    {
        redisManager = RedisManager.GetRedisManagerEnv();
        VisualElement container = uIDocument.rootVisualElement.Q<VisualElement>("game-information");

        await VerPartidas(container, PlayerPrefs.GetInt("jugadorID"));
    }

    async Task VerPartidas(VisualElement container, long jugadorID)
    {
        ScrollView scrollView = container.Q<ScrollView>("game-scroll-view");
        scrollView.Clear();

        Jugador jugador = await Task.Run(() =>
        {
            return redisManager.GetPlayer(jugadorID);
        });

        for (int i = 0; i < jugador.casos.Count; i++)
        {
            Caso caso = jugador.casos[i];

            byte[] bytesImage = redisManager.GetImage($"jugadores:{jugadorID}:caso:{i+1}:imagen");

            Texture2D texture = new (1, 1);
            texture.LoadImage(bytesImage);

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
            scrollView.Add(containerGame);
        }
    }
}