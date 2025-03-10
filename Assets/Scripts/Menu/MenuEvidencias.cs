using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities.Extensions;

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
            if (!Evidencias && !menuPersonajes.Personajes)
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

            objetoPrefab.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].evidencias[i].nombre;
            objetoPrefab.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = Jugador.jugador.casos[Jugador.indexCaso].evidencias[i].tipo;
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

        datosEvidencia.transform.GetChild(0).SetActive(true);
        datosEvidencia.transform.GetChild(1).SetActive(false);
        datosEvidencia.transform.GetChild(2).SetActive(false);
        datosEvidencia.transform.GetChild(3).SetActive(false);
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

        datosEvidencia.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.nombre;
        datosEvidencia.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.descripcion;
        datosEvidencia.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.tipo;
        datosEvidencia.transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = evidencia.analizada ? evidencia.analisis : "No analizada"; 

        datosEvidencia.transform.GetChild(0).SetActive(true);
        datosEvidencia.transform.GetChild(1).SetActive(true);
        datosEvidencia.transform.GetChild(2).SetActive(true);
        datosEvidencia.transform.GetChild(3).SetActive(true);
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
