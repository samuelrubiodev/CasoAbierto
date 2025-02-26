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

    private VaultTransit vaultTransit;
    private SQLiteManager sqliteManager;
    private RedisManager redisManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        sqliteManager = new SQLiteManager();
        sqliteManager.crearConexion();
        vaultTransit = new VaultTransit();

        redisManager = new RedisManager(
            await vaultTransit.DecryptAsync("api-key-encrypt", sqliteManager.GetServers()[Server.REDIS].ipServer), "6379", "",
            await vaultTransit.EncryptAsync("api-key-encrypt", sqliteManager.GetServers()[Server.REDIS].password));

        redisManager.crearConexion();
        long jugadorID = sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer;
        Jugador jugador = redisManager.getJugador(jugadorID);

        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100 * jugador.casos.Count);

        for (int i = 0; i < jugador.casos.Count; i++)
        {
            GameObject button = Instantiate(buttonPrefab, content.transform);
            button.name = jugador.casos[i].idCaso;
            button.GetComponentInChildren<TextMeshProUGUI>().text = jugador.casos[i].tituloCaso;
            button.SetActive(true);

            button.GetComponent<Button>().onClick.AddListener(() => CargarPartida(jugador,i.ToString()));
        }
    }

    public void CargarPartida(Jugador jugador, string indexCaso)
    {
        Jugador.jugador = jugador;
        Jugador.indexCaso = int.Parse(indexCaso);
        SceneManager.LoadScene("SampleScene");
    }

    public void CrearPartida()
    {
        SceneManager.LoadScene("PantallaCarga");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
