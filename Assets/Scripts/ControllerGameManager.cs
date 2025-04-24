using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerGameManager
{
    public void Veredict(JObject jsonGameStatus)
    {
        bool haGanadoUsuario = jsonGameStatus["haGanadoUsuario"].ToObject<bool>();

        FinalSceneManager.isUserWin = haGanadoUsuario;
        SceneManager.LoadScene("FinalScene");
    }
}
