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
            GameObject panelCaso = Instantiate(casoPrefab, content.transform);

            panelCaso.name = "Caso: " + i;

            panelCaso.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = jugador.casos[i].tituloCaso;
            panelCaso.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = jugador.casos[i].lugar;
            panelCaso.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = jugador.casos[i].dificultad;
            panelCaso.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = jugador.casos[i].tiempoRestante;

            panelCaso.SetActive(true);
            panelCaso.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => CargarPartida(jugador, index.ToString()));
            panelCaso.transform.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => CargarPartida(jugador, index.ToString()));
        }
    }

    public void CargarPartida(Jugador jugador, string indexCaso)
    {
        Jugador.jugador = jugador;
        Jugador.indexCaso = int.Parse(indexCaso);
        
        GameObject datosCaso = panelIzquierda.transform.GetChild(1).gameObject;

        datosCaso.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = jugador.casos[int.Parse(indexCaso)].tituloCaso;
        datosCaso.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = jugador.casos[int.Parse(indexCaso)].descripcion;
        datosCaso.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = jugador.casos[int.Parse(indexCaso)].dificultad;
        datosCaso.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = jugador.casos[int.Parse(indexCaso)].tiempoRestante;

        datosCaso.transform.GetChild(0).SetActive(true);
        datosCaso.transform.GetChild(1).SetActive(true);
        datosCaso.transform.GetChild(2).SetActive(true);
        datosCaso.transform.GetChild(3).SetActive(true);
        datosCaso.transform.GetChild(4).SetActive(true);
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
