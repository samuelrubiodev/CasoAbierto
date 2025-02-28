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

        if (sqliteManager.ExistsTable("Player"))
        {
            await VerPartidas(sqliteManager, redisManager, rt);
        }
    }

    async Task CrearConexion()
    {
        redisManager = await RedisManager.GetRedisManager();
    }

    async Task VerPartidas(SQLiteManager sqliteManager, RedisManager redisManager, RectTransform rt)
    {
        Jugador jugador = await Task.Run(() =>
        {
            long jugadorID = sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer;
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
