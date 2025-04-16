using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameInitializer : MonoBehaviour 
{
    public AudioMixer audioMixer;    
    async void Start()
    {
        VaultTransit vaultTransit = new ();

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
        
        ApiKey.API_KEY_OPEN_ROUTER = await vaultTransit.GetKey("OPEN_ROUTER");
        ApiKey.API_KEY_GROQ = await vaultTransit.GetKey("GROQ");
        ApiKey.API_KEY_ELEVENLABS = await vaultTransit.GetKey("ELEVENLABS");
        ApiKey.API_KEY_TOGETHER = await vaultTransit.GetKey("TOGETHER");

        Server.IP_SERVER_REDIS = ConfigEnv.GetEnv(ConfigEnv.Envs.REDIS_HOST);
        Server.CONTRASENA_REDIS = ConfigEnv.GetEnv(ConfigEnv.Envs.REDIS_PASSWORD);
    }
}