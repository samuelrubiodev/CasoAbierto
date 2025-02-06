using OpenAI.Chat;
using OpenAI;
using System;
using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using UnityEditor.EditorTools;

public class MenuInicial : MonoBehaviour
{
    TMP_Dropdown dropbownMicrofonos;

    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
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

        TMP_InputField elevenLabsInput = elevenLabs.GetComponent<TMP_InputField>();
        TMP_InputField openRouterInput = openRouter.GetComponent<TMP_InputField>();


        SQLiteManager sqLiteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqLiteManager.crearConexion();

        VaultTransit vaultTransit = new VaultTransit();

        if (sqLiteManager.ExistsTable("ApiKeys"))
        {
            List<ApiKey> apiKeys = sqLiteManager.GetTable<ApiKey>("SELECT * FROM ApiKeys");
            foreach (ApiKey apiKey in apiKeys)
            {
                if (apiKey.name == "ElevenLabs")
                {
                    elevenLabsInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", apiKey.apiKey);
                }
                else if (apiKey.name == "OpenRouter")
                {
                    openRouterInput.text = await vaultTransit.DecryptAsync("api-key-encrypt", apiKey.apiKey);
                }
            }
        }
        sqLiteManager.cerrarConexion();
    }


    public async void guardarAPis()
    { 
        GameObject elevenLabs = GameObject.Find("inputElevenLabs");
        GameObject openRouter = GameObject.Find("inputOpenRouter");

        TMP_InputField elevenLabsInput = elevenLabs.GetComponent<TMP_InputField>();
        TMP_InputField openRouterInput = openRouter.GetComponent<TMP_InputField>();

        var vaultTransit = new VaultTransit();

        SQLiteManager sqLiteManager = new SQLiteManager(Application.persistentDataPath + "/database.db");
        sqLiteManager.crearConexion();

        if (elevenLabsInput.text != "" && openRouterInput.text != "")
        {
            if (!sqLiteManager.ExistsTable("ApiKeys"))
            {
                sqLiteManager.CreateTable<ApiKey>();

                string apiKeyElevenLabs = await vaultTransit.EncryptAsync("api-key-encrypt", elevenLabsInput.text);
                string apiKeyOpenRouter = await vaultTransit.EncryptAsync("api-key-encrypt", openRouterInput.text);

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

                sqLiteManager.Insert(objetoElevenLabs);
                sqLiteManager.Insert(objetoOpenRouter);
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
                    sqLiteManager.update(apiKey);
                }
            }
        }
        sqLiteManager.cerrarConexion();
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

    public void pruebaOpenAI()
    {
        try
        { 
            OpenAIClientOptions openAIClientOptions = new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            };

            OpenAIClient client = new OpenAIClient(new ApiKeyCredential("[API_REMOVED]"), openAIClientOptions);

            List<ChatMessage> messages = new List<ChatMessage>
            {
                new SystemChatMessage(@"[Contexto del Juego]
                    Estás desarrollando un juego de investigación policial llamado ""Caso Abierto"". El jugador asume el rol de un detective encargado de resolver casos mediante interrogatorios a sospechosos y el análisis de evidencias. El juego se desarrolla en una sala de interrogatorios con interacción verbal y gestión de tiempo.

                    [Creatividad y Realismo]
                    Los casos deben ser realistas, variados y originales, pero siempre dentro de los límites de lo posible en un entorno policial o criminalístico. Evita cualquier elemento de ciencia ficción, paranormal o sobrenatural. Los misterios deben resolverse con lógica, deducción y evidencia.

                    [Importante]
                    
                    El jugador es el investigador, es un investigador de la policía que ha sido asignado para llevar la investigación del caso.
                "),
                new UserChatMessage("Generame un nuevo caso con el nombre de jugador llamado: 'Carlos Santana'")
            };

            string jsonSchema = @"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""datosJugador"": {
                        ""type"": ""object"",
                        ""properties"": {
                             ""nombre"": { ""type"": ""string"", ""description"": ""Nombre del jugador"" },
                             ""estado"": { ""type"": ""string"", ""description"": ""Estado del jugador, Activo o Inactivo"" },
                             ""progreso"": { ""type"": ""string"", ""description"": ""En que caso va, nombre del caso"" }
                         },
                         ""required"": [""nombre"",""estado"",""progreso""],
                         ""additionalProperties"": false
                    },
                    ""Caso"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""tituloCaso"": { ""type"": ""string"", ""description"": ""Titulo del caso"" },
                            ""descripcionCaso"": { ""type"": ""string"", ""description"": ""Descripción del caso"" },
                            ""dificultad"": { ""type"": ""string"", ""description"": ""Facil, Medio o Dificil"" },
                            ""fechaOcurrido"": { ""type"": ""string"", ""description"": ""YYYY-MM-DD"" },
                            ""lugar"": { ""type"": ""string"", ""description"": ""Lugar en el que ha ocurrido el caso"" },
                            ""tiempoRestante"": { ""type"": ""string"", ""description"": ""HH:MM"" },
                            ""cronologia"": { ""type"": ""array"", ""items"": {
                                    ""type"": ""object"",
                                        ""properties"": {
                                            ""fecha"": {""type"": ""string"", ""description"": ""YYYY-MM-DD""},
                                            ""hora"": {""type"": ""string"", ""description"": ""HH:MM""},
                                            ""evento"": {""type"": ""string"", ""description"": ""Descripcion breve del evento""}
                                        },
                                        ""required"": [""fecha"",""hora"",""evento""],
                                        ""additionalProperties"": false
                                    } 
                            },
                            ""evidencias"": {""type"": ""array"", ""items"": {
                                        ""type"": ""object"",
                                        ""properties"": {
                                            ""nombre"": {""type"": ""string"", ""description"": ""Nombre de la evidencia, objeto""},
                                            ""descripcion"": {""type"": ""string"", ""description"": ""Una carta manchada de sangre, un cuchillo con huellas dactilares, una foto de la víctima con un mensaje amenazante, un diario con una página arrancada, etc..""},
                                            ""analisis"": {""type"": ""string"", ""description"": ""Un análisis de la evidencia, puede ser una descripción de lo que se encontró, una conclusión de lo que significa, etc..""},
                                            ""tipo"": {""type"": ""string"", ""description"": ""Arma, Documento, Objeto personal, Foto, Video, etc..""},
                                            ""ubicacion"": {""type"": ""string""}
                                        },
                                        ""required"": [""nombre"",""descripcion"",""analisis"",""tipo"",""ubicacion""],
                                        ""additionalProperties"": false
                                    }
                            },
                            ""personajes"": {""type"": ""array"", ""items"": {
                                        ""type"": ""object"",
                                        ""properties"": {
                                            ""nombre"": {""type"": ""string"", ""description"": ""Nombre del personaje""},
                                            ""rol"": {""type"": ""string"", ""description"": ""Testigo, Victima, Cómplice, Informante, Periodista, Familia del sospechoso, etc..""},
                                            ""estado"": {""type"": ""string"", ""description"": ""Vivo, Muerto o Desaparecido""},
                                            ""descripcion"": {""type"":""string"", ""description"": ""Ejemplo: Un hombre mayor con un aire autoritario y una cicatriz prominente en la mejilla. Una mujer joven con gafas grandes y un nerviosismo evidente al hablar. Una ancina amable pero con un comportamiento claramente evasivo. Una joven madre que abraza una foto familiar mientras habla contigo, etc..""},
                                            ""estado_emocional"": {""type"": ""string"", ""description"": ""Nervioso,Tranquilo,Confiado,Arrogante,Asustado,Confuso,Defensivo,Culpable,etc..""}
                                        },
                                        ""required"": [""nombre"",""rol"",""estado"",""descripcion"",""estado_emocional""],
                                        ""additionalProperties"": false
                                    }
                            },
                            ""explicacionCasoResuelto"": {""type"": ""string"", ""description"": ""Descripcion de como se podría resolver el caso""}
                        },
                        ""required"": [""tituloCaso"",""descripcionCaso"",""dificultad"",""fechaOcurrido"",""lugar"",""tiempoRestante"",""cronologia"",""evidencias"",""personajes"",""explicacionCasoResuelto""],
                        ""additionalProperties"": false
                    }
                },
                ""required"": [""datosJugador"",""Caso""],
                ""additionalProperties"": false
            }";

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "caso_data",
                    jsonSchema: BinaryData.FromString(jsonSchema),
                    jsonSchemaIsStrict: true),
            };

            ChatCompletion completion = client.GetChatClient("google/gemini-2.0-flash-001").CompleteChat(messages, options);

            using JsonDocument jsonDocument = JsonDocument.Parse(completion.Content[0].Text);

            Debug.Log(jsonDocument.RootElement.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void Salir()
    {
        Application.Quit();
    }
}
