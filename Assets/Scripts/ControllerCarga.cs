using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerCarga : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SQLiteManager sqLiteManager;
    VaultTransit vaultTransit;
    async void Start()
    {
        sqLiteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        vaultTransit = new VaultTransit();

        sqLiteManager.crearConexion();
        RedisManager redisManger = new RedisManager(
            await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].ipServer), "6379", "",
            await vaultTransit.EncryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].password));

        await CrearCaso(redisManger, vaultTransit, await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey));
        Debug.Log("Caso creado correctamente");
        SceneManager.LoadScene("SampleScene");
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

}
