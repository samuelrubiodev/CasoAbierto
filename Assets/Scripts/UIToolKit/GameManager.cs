using System.Threading.Tasks;
using UnityEngine;
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
            
        }
    }
}