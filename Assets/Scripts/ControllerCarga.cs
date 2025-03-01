using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerCarga : MonoBehaviour
{
    public static bool tieneCaso = false;

    SQLiteManager sqLiteManager;
    VaultTransit vaultTransit;
    async void Start()
    {
        sqLiteManager = SQLiteManager.GetSQLiteManager();
        vaultTransit = new ();

        await ConectarApis();

        if (tieneCaso)
        {
            SceneManager.LoadScene("SampleScene");
            return;
        }
        else
        {
            RedisManager redisManger = await RedisManager.GetRedisManager();
            long jugadorID = GetJugadorID();

            await CrearCaso(redisManger, vaultTransit, ApiKey.API_KEY_OPEN_ROUTER,jugadorID);
            
            if (Inicializacion.jugadorID != -1)
            {
                PlayerPrefs.SetInt("jugadorID", (int)Inicializacion.jugadorID);
                PlayerPrefs.Save();
            }  

            await CargarPartida(redisManger, GetJugadorID());
            SceneManager.LoadScene("SampleScene");
        }
    }

    async Task CargarPartida(RedisManager redisManager, long jugadorID)
    {
        Jugador jugador = await Task.Run(() =>
        {
            return redisManager.getJugador(jugadorID);
        });
        
        for(int i = 0; i < jugador.casos.Count; i++)
        {
            if(jugador.casos[i].idCaso == Inicializacion.idCasoGenerado.ToString())
            {
                Jugador.jugador = jugador;
                Jugador.indexCaso = i;
                return;
            }
        }
    }
    
    async Task CrearCaso(RedisManager redisManager, VaultTransit vaultTransit, string apiKeyOpenRouter, long jugadorID)
    {
        await Task.Run(async () =>
        {
            Inicializacion inicializacion = new("Samuel");

            inicializacion.setSQliteManager(sqLiteManager);
            inicializacion.setRedisManager(redisManager);
            inicializacion.setVaultTransit(vaultTransit);
            inicializacion.setApiKeyOpenRouter(apiKeyOpenRouter);

            await inicializacion.crearBaseDatosRedis(jugadorID);
        });
    }

    async Task ConectarApis()
    {
        ApiKey.API_KEY_OPEN_ROUTER = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey);
        ApiKey.API_KEY_GROQ = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.GROQ].apiKey);
        ApiKey.API_KEY_ELEVENLABS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.ELEVENLABS].apiKey);

        Server.IP_SERVER_REDIS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].ipServer);
        Server.CONTRASENA_REDIS = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].password);
    }

    private long GetJugadorID()
    {
        if (PlayerPrefs.HasKey("jugadorID"))
            return PlayerPrefs.GetInt("jugadorID");
        else
            return -1;
    }
}
