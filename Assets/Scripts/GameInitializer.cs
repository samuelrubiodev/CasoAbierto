using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Utilities.Extensions;

public class GameInitializer : MonoBehaviour 
{
    public AudioMixer audioMixer;
    public ErrorOverlayUI errorOverlayUI;    
    
    async void Start()
    {
        VaultTransit vaultTransit = new ();

        CallBackButton tryButton = OnTryButtonClicked;
        errorOverlayUI.SetTryCallBackButtonDelegate(tryButton);
        errorOverlayUI.mainButton.RemoveFromHierarchy();

        var settingsLoaders = new List<IGameSettingsLoader>
        {
            new AudioSettingsLoader(audioMixer),
            new ScreenSettingsLoader(),
            new GraphicsSettingsLoader()
        };

        foreach (var loader in settingsLoaders)
        {
            loader.LoadSettings();
        }

        try {
            new InternetConnection().CheckInternetConnection();
            ApiKey.API_KEY_OPEN_ROUTER = await vaultTransit.GetKey("OPEN_ROUTER");
            ApiKey.API_KEY_GROQ = await vaultTransit.GetKey("GROQ");
            ApiKey.API_KEY_ELEVENLABS = await vaultTransit.GetKey("ELEVENLABS");
            Server.ACTIVE_CASE_HOST = ConfigEnv.GetEnv(ConfigEnv.Envs.ACTIVE_CASE_HOST);

            GenerationIDHttpRequest generationIDHttpRequest = new ();
            ElevenLabsHttpRequest elevenLabsHttpRequest = new ();
            
            JObject jsonOpenRouterResponse = await generationIDHttpRequest.GetAsync("credits");
            JObject jsonElevenLabsResponse = await elevenLabsHttpRequest.GetAsync("user");
            
            OpenRouterImpl openRouterImpl = OpenRouterImpl.Instance;
            ElevenLabsImpl elevenLabsImpl = ElevenLabsImpl.Instance;

            await openRouterImpl.GetActualCreditsBalance(jsonOpenRouterResponse);
            await elevenLabsImpl.GetActualCreditsBalance(jsonElevenLabsResponse);

            Debug.Log("Creditos restantes: " + openRouterImpl.ActualCreditsBalance);
            Debug.Log("Caracteres restantes: " + elevenLabsImpl.ActualCharacterCount);

        } catch (KeyNotFoundException) 
        {
            GetComponent<MainMenu>().SetActive(false);

            UtilitiesErrorMessage errorMessage = new (Application.dataPath + "/Resources/ErrorMessages/ErrorMessage.json");
            ErrorsMessage errors = errorMessage.ReadJSON();

            System.Random random = new ();
            int randomTitle = random.Next(0, errors.conexion.titulos.Count);
            int randomMessage = random.Next(0, errors.conexion.mensajes.Count);

            errorOverlayUI.ShowError(errors.conexion.titulos[randomTitle], errors.conexion.mensajes[randomMessage]);
            return;
        }
        catch (InternetConnectionNotFound) 
        {
            GetComponent<MainMenu>().SetActive(false);
            errorOverlayUI.ShowError("Ups, ¿Has cortado los cables?", "No se ha podido utilizar los cables");
            return;
        }
    }

    public void OnTryButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }
}