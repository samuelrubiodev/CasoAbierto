using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerCarga : MonoBehaviour
{
    public static bool tieneCaso = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

            await CrearCaso(redisManger, vaultTransit, await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey));
            await CargarPartida(sqLiteManager, redisManger);
            SceneManager.LoadScene("SampleScene");
        }
    }

    async Task CargarPartida(SQLiteManager sqliteManager, RedisManager redisManager)
    {
        Jugador jugador = await Task.Run(() =>
        {
            long jugadorID = sqliteManager.GetTable<Player>("SELECT * FROM Player")[0].idPlayer;
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
    
    async Task CrearCaso(RedisManager redisManager, VaultTransit vaultTransit, string apiKeyOpenRouter)
    {
        await Task.Run(async () =>
        {
            sqLiteManager.crearConexion();
            Inicializacion inicializacion = new("Samuel");

            inicializacion.setSQliteManager(sqLiteManager);
            inicializacion.setRedisManager(redisManager);
            inicializacion.setVaultTransit(vaultTransit);
            inicializacion.setApiKeyOpenRouter(apiKeyOpenRouter);

            await inicializacion.crearBaseDatosRedis();
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

}
