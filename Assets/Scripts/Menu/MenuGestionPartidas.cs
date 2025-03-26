using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Utilities.Extensions;
using UnityEngine.EventSystems;

public class MenuGestionPartidas : MonoBehaviour
{
    public GameObject content;
    public GameObject casoPrefab;
    public GameObject panelIzquierda;
    private SQLiteManager sqliteManager;
    private RedisManager redisManager;
    public static GameObject casoSeleccionado;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
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
            return redisManager.GetPlayer(jugadorID);
        });

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100 * jugador.casos.Count);

        for (int i = 0; i < jugador.casos.Count; i++)
        {
            int index = i;
            GameObject panelCaso = Instantiate(casoPrefab, content.transform);

            panelCaso.name = "Caso: " + i;

            Util.LoadText(panelCaso, jugador.casos[i].GetSimpleDataStrings());

            panelCaso.SetActive(true);
            panelCaso.transform.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => CargarPartida(jugador, index.ToString()));
        }
    }

    public void CargarPartida(Jugador jugador, string indexCaso)
    {
        Jugador.jugador = jugador;
        Jugador.indexCaso = int.Parse(indexCaso);
        
        GameObject datosCaso = panelIzquierda.transform.GetChild(1).gameObject;

        Caso caso = jugador.casos[int.Parse(indexCaso)];
        Util.LoadText(datosCaso, caso.GetDataStrings());
        Util.LoadBool(datosCaso, new bool[] { true, true, true, true, true });
    }

    public void JugarPartida()
    {
        ControllerCarga.tieneCaso = true;
        SceneManager.LoadScene("PantallaCarga");
    }

    public void CrearPartida()
    {
        ControllerCarga.tieneCaso = false;
        SceneManager.LoadScene("PantallaCarga");
    }
}
