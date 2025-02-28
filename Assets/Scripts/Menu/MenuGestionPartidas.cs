using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuGestionPartidas : MonoBehaviour
{
    public GameObject content;
    public GameObject buttonPrefab;
    private SQLiteManager sqliteManager;
    private RedisManager redisManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        VaultTransit.CargarConfiguracion();
        sqliteManager = SQLiteManager.GetSQLiteManager();

        await CrearConexion();
        RectTransform rt = content.GetComponent<RectTransform>();

        if (PlayerPrefs.HasKey("jugadorID"))
        {
            await VerPartidas(redisManager, rt, PlayerPrefs.GetInt("jugadorID"));
        }
    }

    async Task CrearConexion()
    {
        redisManager = await RedisManager.GetRedisManager();
    }

    async Task VerPartidas(RedisManager redisManager, RectTransform rt, long jugadorID)
    {
        Jugador jugador = await Task.Run(() =>
        {
            return redisManager.getJugador(jugadorID);
        });

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100 * jugador.casos.Count);

        for (int i = 0; i < jugador.casos.Count; i++)
        {
            int index = i;
            GameObject button = Instantiate(buttonPrefab, content.transform);
            button.name = jugador.casos[i].idCaso;
            button.GetComponentInChildren<TextMeshProUGUI>().text = jugador.casos[i].tituloCaso;
            button.SetActive(true);

            button.GetComponent<Button>().onClick.AddListener(() => CargarPartida(jugador, i.ToString()));
        }
    }

    public void CargarPartida(Jugador jugador, string indexCaso)
    {
        Jugador.jugador = jugador;
        Jugador.indexCaso = int.Parse(indexCaso) - 1;
        ControllerCarga.tieneCaso = true;
        SceneManager.LoadScene("PantallaCarga");
    }

    public void CrearPartida()
    {
        ControllerCarga.tieneCaso = false;
        SceneManager.LoadScene("PantallaCarga");
    }
}
