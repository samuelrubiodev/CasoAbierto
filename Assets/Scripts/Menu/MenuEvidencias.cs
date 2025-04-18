using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEvidencias : MonoBehaviour
{
    public GameObject content;
    public GameObject scrollView;
    public GameObject evidenciaPrefab;
    public GameObject panel;
    public GameObject panelIzquierda;
    public FirstPersonController FirstPersonController;
    public bool Evidencias = false;
    public static Evidencia evidenciaSeleccionada;

    public MenuPersonajes menuPersonajes;
    public APIRequest aPIRequest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!Evidencias && !menuPersonajes.Personajes && !ControllerGame.estaEscribiendo)
            {
                MostrarEvidencias();
            }
            else
            {
                OcultarEvidencias();
            }
        }
    }

    public void MostrarEvidencias()
    {
        FirstPersonController.enabled = false;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RectTransform rt = content.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 100 * Jugador.jugador.casos[Jugador.indexCaso].evidencias.Count);

        for (int i = 0; i < Jugador.jugador.casos[Jugador.indexCaso].evidencias.Count; i++)
        {
            int index = i;
            GameObject objetoPrefab = Instantiate(evidenciaPrefab, content.transform);

            Util.LoadText(objetoPrefab, Jugador.jugador.casos[Jugador.indexCaso].evidencias[i].GetSimpleDataStrings());

            objetoPrefab.transform.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => 
                { 
                    CargarEvidencia(Jugador.jugador.casos[Jugador.indexCaso].evidencias[index]); 
                });

            objetoPrefab.SetActive(true);
        }
        
        scrollView.SetActive(true);
        Evidencias = true;
        panel.SetActive(true);
        panelIzquierda.SetActive(true);

        GameObject datosEvidencia = panelIzquierda.transform.GetChild(1).gameObject;

        Util.LoadBool(datosEvidencia, new bool[] { true, false, false, false });
    }

    public async void AnalizarEvidencia()
    {
        if (!evidenciaSeleccionada.analizada)
        {
            string json = await aPIRequest.AnalizarEvidencia(evidenciaSeleccionada);
            JObject jobject = JObject.Parse(json);

            GameObject datosEvidencia = panelIzquierda.transform.GetChild(1).gameObject;

            string resultadoAnalisis = jobject["evidencia"]?["resultadoAnalisis"].ToString();

            evidenciaSeleccionada.analisis = resultadoAnalisis;

            datosEvidencia.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = resultadoAnalisis;

            evidenciaSeleccionada.analizada = true;
        }
    }

    public void DeseleccionarEvidencia()
    {
        evidenciaSeleccionada = null;
        OcultarEvidencias();
    }

    public void SeleccionarEvidencia()
    {
        OcultarEvidencias();
    }

    public void CargarEvidencia(Evidencia evidencia)
    {
        evidenciaSeleccionada = evidencia;

        GameObject datosEvidencia = panelIzquierda.transform.GetChild(1).gameObject;
        
        Util.LoadText(datosEvidencia, evidencia.GetDataStrings());
        Util.LoadBool(datosEvidencia, new bool[] { true, true, true, true });
    }

    public void OcultarEvidencias()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        FirstPersonController.enabled = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        scrollView.SetActive(false);
        Evidencias = false;
        panel.SetActive(false);
        panelIzquierda.SetActive(false);
    }
}
