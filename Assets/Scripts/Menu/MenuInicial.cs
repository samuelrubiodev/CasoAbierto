using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuInicial : MonoBehaviour
{
    TMP_Dropdown dropbownMicrofonos;
    SQLiteManager sqLiteManager;
    VaultTransit vaultTransit;

    void Start()
    {
        sqLiteManager = SQLiteManager.GetSQLiteManager();
        vaultTransit = new VaultTransit();
    }

    public async void guardarServidor()
    {
        GameObject servidor = GameObject.Find("inputServidorRedis");
        TMP_InputField servidorInput = servidor.GetComponent<TMP_InputField>();

        GameObject contrasenaRedis = GameObject.Find("inputContrasenaRedis");
        TMP_InputField contrasenaRedisInput = contrasenaRedis.GetComponent<TMP_InputField>();
        
        if (servidorInput.text != "" && contrasenaRedisInput.text != "")
        {
            if (!sqLiteManager.ExistsTable("Server"))
            {
                sqLiteManager.CreateTable<Server>();
                var objeto = new Server
                {
                    nombreServicio = "Redis",
                    ipServer = await vaultTransit.EncryptAsync("api-key-encrypt", servidorInput.text),
                    portServer = 6379,
                    nombreUsuario = "",
                    password = await vaultTransit.EncryptAsync("api-key-encrypt", contrasenaRedisInput.text)
                };
                sqLiteManager.Insert(objeto);
            }
            else
            {
                List<Server> servidores = sqLiteManager.GetTable<Server>("SELECT * FROM Server");
                foreach (Server servidorRedis in servidores)
                {
                    if (servidorRedis.nombreServicio == "Redis")
                    {
                        servidorRedis.ipServer = await vaultTransit.EncryptAsync("api-key-encrypt", servidorInput.text);
                        servidorRedis.password = await vaultTransit.EncryptAsync("api-key-encrypt", contrasenaRedisInput.text);
                        sqLiteManager.update(servidorRedis);
                        break;
                    }
                }
            }
        }
    }

    public async void listarServidor()
    {
        GameObject servidor = GameObject.Find("inputServidorRedis");
        TMP_InputField servidorInput = servidor.GetComponent<TMP_InputField>();
        GameObject contrasenaRedis = GameObject.Find("inputContrasenaRedis");
        TMP_InputField contrasenaRedisInput = contrasenaRedis.GetComponent<TMP_InputField>();
        if (sqLiteManager.ExistsTable("Server"))
        {
            servidorInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].ipServer);
            contrasenaRedisInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetServers()[Server.REDIS].password);
        }
    }

    public void mostrarContrasenaRedis()
    {
        GameObject contrasenaRedis = GameObject.Find("inputContrasenaRedis");
        TMP_InputField contrasenaRedisInput = contrasenaRedis.GetComponent<TMP_InputField>();
        if (contrasenaRedisInput.text != "")
        {
            if (contrasenaRedisInput.contentType == TMP_InputField.ContentType.Password)
            {
                contrasenaRedisInput.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                contrasenaRedisInput.contentType = TMP_InputField.ContentType.Password;
            }
            contrasenaRedisInput.ForceLabelUpdate();
        }
    }


    // MENU AUDIO

    public void listadoMicrofonosDisponibles()
    {
        GameObject objeto = GameObject.Find("DropbownMicrofono");

        GameObject porcentajeVolGeneral = GameObject.Find("SliderVolGeneral");
        GameObject porcentajeVolMusica = GameObject.Find("SliderVolMusica");

        Slider sliderVolGeneral = porcentajeVolGeneral.GetComponent<Slider>();
        Slider sliderVolMusica = porcentajeVolMusica.GetComponent<Slider>();

        sliderVolGeneral.onValueChanged.AddListener((value) => MostrarSlider(value, "porcentajeVolGeneral"));
        sliderVolMusica.onValueChanged.AddListener((value) => MostrarSlider(value, "porcentajeVolMusica"));

        TMP_Dropdown dropbown = objeto.GetComponent<TMP_Dropdown>();
        dropbownMicrofonos = dropbown;

        dropbown.options.Clear();

        string[] microfonos = Microphone.devices;

        for (int i = 0; i < microfonos.Length; i++)
        {
            dropbown.options.Add(new TMP_Dropdown.OptionData(name = microfonos[i]));
        }

        if (PlayerPrefs.HasKey("microfono"))
        {
            dropbown.value = dropbown.options.FindIndex((i) => { return i.text == PlayerPrefs.GetString("microfono"); });
        }

        dropbown.onValueChanged.AddListener(DropBownMicrofonoListener);
    }

    private void MostrarSlider(float arg0, string porcentajeNombre)
    {
        GameObject porcentajeVol = GameObject.Find(porcentajeNombre);
        porcentajeVol.GetComponent<TMP_Text>().text = arg0.ToString("0") + "%";
    }

    private void DropBownMicrofonoListener(int newValue)
    {
        PlayerPrefs.SetString("microfono", dropbownMicrofonos.options[newValue].text);
    }

    public void guardarConfiguracionVolumen()
    {
        GameObject volumenGeneral = GameObject.Find("SliderVolGeneral");
        GameObject volumenMusica = GameObject.Find("SliderVolMusica");

        Slider sliderVolumenGeneral = volumenGeneral.GetComponent<Slider>();
        Slider sliderVolumenMusica = volumenMusica.GetComponent<Slider>();

        PlayerPrefs.SetFloat("volumenGeneral", sliderVolumenGeneral.value);
        PlayerPrefs.SetFloat("volumenMusica", sliderVolumenMusica.value);
    }

    public void menuVolumen()
    {
        GameObject volumenGeneral = GameObject.Find("SliderVolGeneral");
        GameObject volumenMusica = GameObject.Find("SliderVolMusica");

        Slider sliderVolumenGeneral = volumenGeneral.GetComponent<Slider>();
        Slider sliderVolumenMusica = volumenMusica.GetComponent<Slider>();

        if (PlayerPrefs.HasKey("volumenGeneral"))
        {
            sliderVolumenGeneral.value = PlayerPrefs.GetFloat("volumenGeneral");
        }

        if (PlayerPrefs.HasKey("volumenMusica"))
        {
            sliderVolumenMusica.value = PlayerPrefs.GetFloat("volumenMusica");
        }
    }

    // MENU API

    public async void listarApis()
    {
        GameObject elevenLabs = GameObject.Find("inputElevenLabs");
        GameObject openRouter = GameObject.Find("inputOpenRouter");
        GameObject groq = GameObject.Find("inputGroq");

        TMP_InputField elevenLabsInput = elevenLabs.GetComponent<TMP_InputField>();
        TMP_InputField openRouterInput = openRouter.GetComponent<TMP_InputField>();
        TMP_InputField groqInput = groq.GetComponent<TMP_InputField>();

        SQLiteManager sqLiteManager = SQLiteManager.GetSQLiteManager();

        if (sqLiteManager.ExistsTable("ApiKeys"))
        {
            elevenLabsInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.ELEVENLABS].apiKey);
            openRouterInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.OPEN_ROUTER].apiKey);
            groqInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", sqLiteManager.GetAPIS()[ApiKey.GROQ].apiKey);
        }
    }


    public async void guardarAPis()
    { 
        GameObject elevenLabs = GameObject.Find("inputElevenLabs");
        GameObject openRouter = GameObject.Find("inputOpenRouter");
        GameObject groq = GameObject.Find("inputGroq");

        TMP_InputField elevenLabsInput = elevenLabs.GetComponent<TMP_InputField>();
        TMP_InputField openRouterInput = openRouter.GetComponent<TMP_InputField>();
        TMP_InputField groqInput = groq.GetComponent<TMP_InputField>();

        SQLiteManager sqLiteManager = SQLiteManager.GetSQLiteManager();

        if (elevenLabsInput.text != "" && openRouterInput.text != "")
        {
            if (!sqLiteManager.ExistsTable("ApiKeys"))
            {
                sqLiteManager.CreateTable<ApiKey>();

                string apiKeyElevenLabs = await vaultTransit.EncryptAsync("api-key-encrypt", elevenLabsInput.text);
                string apiKeyOpenRouter = await vaultTransit.EncryptAsync("api-key-encrypt", openRouterInput.text);
                string apiKeyGroq = await vaultTransit.EncryptAsync("api-key-encrypt", groqInput.text);

                var objetoElevenLabs = new ApiKey
                {
                    name = "ElevenLabs",
                    apiKey = apiKeyElevenLabs
                };

                var objetoOpenRouter = new ApiKey
                {
                    name = "OpenRouter",
                    apiKey = apiKeyOpenRouter
                };

                var objetoGroq = new ApiKey
                {
                    name = "Groq",
                    apiKey = apiKeyGroq
                };

                sqLiteManager.Insert(objetoElevenLabs);
                sqLiteManager.Insert(objetoOpenRouter);
                sqLiteManager.Insert(objetoGroq);
            }
            else 
            {
                List<ApiKey> apiKeys = sqLiteManager.GetTable<ApiKey>("SELECT * FROM ApiKeys");

                foreach (ApiKey apiKey in apiKeys)
                {
                    if (apiKey.name == "ElevenLabs")
                    {
                        apiKey.apiKey = await vaultTransit.EncryptAsync("api-key-encrypt", elevenLabsInput.text);
                    }
                    else if (apiKey.name == "OpenRouter")
                    {
                        apiKey.apiKey = await vaultTransit.EncryptAsync("api-key-encrypt", openRouterInput.text);
                    }
                    else if (apiKey.name == "Groq")
                    {
                        apiKey.apiKey = await vaultTransit.EncryptAsync("api-key-encrypt", groqInput.text);
                    }
                    sqLiteManager.update(apiKey);
                }
            }
        }
    }

    public void mostrarApiElevenLabs()
    {
        GameObject elevenLabs = GameObject.Find("inputElevenLabs");
        TMP_InputField elevenLabsInput = elevenLabs.GetComponent<TMP_InputField>();

        if (elevenLabsInput.text != "")
        {
            if (elevenLabsInput.contentType == TMP_InputField.ContentType.Password)
            {
                elevenLabsInput.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                elevenLabsInput.contentType = TMP_InputField.ContentType.Password;
            }
            elevenLabsInput.ForceLabelUpdate();
        }
    }

    public void mostrarApiOpenRouter()
    {
        GameObject openRouter = GameObject.Find("inputOpenRouter");
        TMP_InputField openRouterInput = openRouter.GetComponent<TMP_InputField>();

        if (openRouterInput.text != "")
        {
            if (openRouterInput.contentType == TMP_InputField.ContentType.Password)
            {
                openRouterInput.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                openRouterInput.contentType = TMP_InputField.ContentType.Password;
            }
            openRouterInput.ForceLabelUpdate();
        }
    }

    public void mostrarAPIGroq()
    {
        GameObject groq = GameObject.Find("inputGroq");
        TMP_InputField groqInput = groq.GetComponent<TMP_InputField>();
        if (groqInput.text != "")
        {
            if (groqInput.contentType == TMP_InputField.ContentType.Password)
            {
                groqInput.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                groqInput.contentType = TMP_InputField.ContentType.Password;
            }
            groqInput.ForceLabelUpdate();
        }
    }

    public void volverMenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Salir()
    {
        Application.Quit();
    }
}
