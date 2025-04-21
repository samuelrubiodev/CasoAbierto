using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerGameManager : MonoBehaviour
{
    public JObject jsonGameStatus { get; set; }
    public JObject jsonEmotionalState { get; set; }

    // Update is called once per frame
    void Update()
    {
        if (jsonGameStatus != null && jsonEmotionalState != null && UIMessageManager.isProcessed)
        {
            UIMessageManager.isProcessed = false;
            bool seHaTerminado = jsonGameStatus["seHaTerminado"].ToObject<bool>();
            bool haGanadoUsuario = jsonGameStatus["haGanadoUsuario"].ToObject<bool>();
    
            if (seHaTerminado && haGanadoUsuario)
            {
                FinalSceneManager.isUserWin = true;
                SceneManager.LoadScene("FinalScene");
            } else if (seHaTerminado && !haGanadoUsuario)
            {
                FinalSceneManager.isUserWin = false;
                SceneManager.LoadScene("FinalScene");
            }
        }
    }
}
